using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.InteropServices.ComTypes;
using System.Collections;

namespace AutoDataUpdate
{
    public partial class mainscreen : Form
    {
        // レジストリキー
        private const string APP_AUTORUN_KEY = @"Software\Microsoft\Windows\CurrentVersion\Run";
//        private const string APP_AUTORUN_ULL_KEY = @"Software\Microsoft\Windows\CurrentVersion\Run\AutoDataUpdate";
        private const string APP_REG_FULL_KEY = @"Software\AutoDataUpdate";
        private const string APP_REG_KEY = @"Software\AutoDataUpdate\system";
        private const string APP_REG_SUBKEY_UPDATE_KEY = "lastUpdate";
        private const string APP_REG_SUBKEY_SAVEPATH_KEY = "savePath";
        private const string APP_REG_SUBKEY_STOPCOUNT_KEY = "stopCount";
        private const string APP_REG_SUBKEY_POWER_AUTO_KEY = "auto";
        // JPX（日本取引所URL）
        private const string JPX_URL = "https://www.jpx.co.jp";
        // JPX 売禁リストをダウンロード出来るURL名(スクレイピング対象サイト)
        private const string JPX_STOP_LIST_URL = "https://www.jpx.co.jp/markets/equities/ss-reg/index.html";
        private const string GET_CSV_NAME = "_Stocks_Restricted_Nextday.csv";
        // PC電源オン時の自動実行パラメータ（command）
        private const string AUTO_EXEC_CMD = "/auto";

        // 売禁データ更新日（未）
        private const string URI_NON_UPADTE = "----/--/--";

        // アクセスURL（売禁データ取得サイトのURL）
        private string accessurl = null;
        // 実行ファイルが格納されているパス
        private string exedir = null;
        // 貸借URLデータのDLフォルダパス
        private string dlDir = null;
        // 貸借URLデータのDLパス
        private string dlPath = null;
        DateTime targetDt = DateTime.Now;
        private string lastUpdateKey = null;
        private string savePathKey = null;
//        private string powerAutoKey = null;
        private int stopCount = 0;

//        private static bool beforeEvent = false;

        public mainscreen()
        {
            InitializeComponent();
        }

        //
        // データ更新処理(ファイル更新)
        //
        private void dateUpdate()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            try
            {
                wc.DownloadFile(accessurl, dlPath);
                wc.Dispose();

            }
            catch (WebException we)
            {
                if (((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    // 現在日付取得
                    DateTime dt = DateTime.Now;
                    string nowDateStr = dt.ToString("yyyy/MM/dd");
                    MessageBox.Show("日本取引所グループ（JPC）サイトで" + nowDateStr + "の売禁データファイルが\nまだ用意されていません。16:30以降に再度実行して下さい",
                        "警告：前日データで更新されました。",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }
            }
            catch (Exception)
            {
                // 処理失敗
                MessageBox.Show("指定URLのファイルダウンロードに失敗しました",
                    "エラーメッセージ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }


            List<ReadCSVFileData> lists = new List<ReadCSVFileData>();
            // 既存ファイル読み込み
            if (readExistingStopList(savePathKey, ref lists) == false)
            {
                // 処理失敗
                MessageBox.Show("指定されたファイル（更新対象ファイル）の読み込みに失敗しました。\n指定されたパスを確認して下さい",
                    "警告",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;

            }
            // データ更新
            if (updateStopList(ref lists) == false)
            {
                // 処理失敗
                MessageBox.Show("指定された売禁ファイルのデータ更新に失敗しました。",
                   "警告",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            // ファイル更新
            if (writeExistingStopList(savePathKey, lists) == false)
            {
                // 処理失敗
                MessageBox.Show("指定された売禁ファイルの更新に失敗しました。",
                    "警告",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            savepath.Text = savePathKey;
            // 処理成功
            lastUpdateText.Text = targetDt.ToString("yyyy/MM/dd");
            MessageBox.Show(lastUpdateText.Text + "分のファイル更新が完了しました。",
                   "正常に更新しました",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information);

        }

        //
        // 画面起動時処理
        //
        private void main_scrennload(object sender, EventArgs e)
        {
            readJPXData();
        }

        //
        // 自動更新処理
        //
        private void autoExecCommand()
        {
//@
            // 現在日付取得
            DateTime dt = DateTime.Now;

STARTPROC_AUTO:
            // ダウンロード可能な日を取得        public int endtDate; // 売禁終了日付
            if (getTargetDate(dt.ToString("yyyy/MM/dd"), ref targetDt) == false)
            {
                return;
            }
            string targetDate = targetDt.ToString("yyyyMMdd");
            // JPXより当日もしくは前営業にの売禁止銘柄一覧を取得する。
            string stopListURL = getJPXScrapingURL(targetDate, JPX_STOP_LIST_URL);
            if (stopListURL.Equals("-1") == true)
            {
                // 前営業日を指定しなおす
                dt = dt.AddDays(-1);
                goto STARTPROC_AUTO;
            }
            else if (stopListURL.Equals("0") == true)
            {
                return;
            }

            accessurl = stopListURL;

            // 実行フォルダのパス取得
            exedir = System.IO.Directory.GetCurrentDirectory();
            // DL先パス作成
            dlDir = exedir + "\\dl";
            dlPath = dlDir + "\\" + targetDt.ToString("yyyyMMdd") + GET_CSV_NAME;
            // フォルダ作成
            Directory.CreateDirectory(dlDir);
            // ダウンロード先フォルダの不要ファイル削除(存在する場合)
            deleteTargetDir(dlDir);

            // レジストリデータの読込み(画面UI更新)
            RegRead();

            savepath.Text = savePathKey;

            // データ更新処理
            dateUpdate();

        }

        //
        // 指定された日付より、取得可能な日付を取得する
        //
        // 引数 : targetDate 現在日付は平日
        // 引数 : refTargetDate 参照渡しでダウンロード対象日を取得する
        //
        private bool getTargetDate(string targetDate, ref DateTime refTargetDate)
        {
　          bool isHolyDayFlag = false;
            short datePosition = 0;

            // ターゲット日付
            DateTime dt = DateTime.Parse(targetDate);

            while (isHolyDayFlag == false)
            {
                // 祝日判定
                dt = dt.AddDays(datePosition);
                datePosition = isJapanHolidayCheck(dt);
                if (datePosition == -100)
                {
                    // エラー処理
                    MessageBox.Show("祝日計算に失敗しました", "エラーメッセージ");
                    return false;
                }

                // データ取得日を繰下げ
                dt = dt.AddDays(datePosition);
                datePosition = 0;

                // 休日判定(土日)
                //                DateTime ss;
                short holchk = isHolidayCheck(dt);
                datePosition = (short)(datePosition + holchk);
                if (datePosition == -100)
                {
                    // エラー処理
                    MessageBox.Show("平日曜日計算に失敗しました", "エラーメッセージ");
                    return false;
                }
                else
                {
                    if (datePosition == 0)
                    {
                        isHolyDayFlag = true;
                    }
                }
            }

            // データ取得日を繰下げ
            dt = dt.AddDays(datePosition);
            refTargetDate = dt;

            return true;

       }

        //
        // 休日判定
        //
        // return  0 : 現在日付は平日
        //        -1 : 土曜日
        //        -2 : 日曜日
        //      -100 : Error
        //
        private short isHolidayCheck(DateTime compDate)
        {
            short ret = -100;

            DayOfWeek dayWeek = compDate.DayOfWeek;
            switch(dayWeek)
            {
                case DayOfWeek.Sunday:      // 日曜日
                    ret = -2;
                    break;
                case DayOfWeek.Saturday:    // 土曜日
                    ret = -1;
                    break;
                case DayOfWeek.Friday:    // 平日
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Thursday:
                case DayOfWeek.Wednesday:
                    ret = 0;
                    break;
                default:
                    break;
            }

            return ret;

        }

        //
        // 祝日判定
        //
        // return  0 : 現在日付は平日
        //        -1 : 土曜日
        //        -2 : 日曜日
        //      -100 : Error
        //
        private short isJapanHolidayCheck(DateTime compDate)
        {
            short ret = 0;
            Dictionary<DateTime, string> dic = new Dictionary<DateTime, string>();

            // ファイルの中身を取得
            string _path = @"https://www8.cao.go.jp/chosei/shukujitsu/shukujitsu.csv";

            try
            {
                var client = new System.Net.WebClient();
                byte[] buffer = client.DownloadData(_path);
                string str = Encoding.GetEncoding("shift_jis").GetString(buffer);

                // 行毎に配列に分割
                string[] rows = str.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                // 一行目を飛ばしてデータをディクショナリに格納
                rows.Skip(1).ToList().ForEach(row =>
                {
                    var cols = row.Split(',');
                    dic.Add(DateTime.Parse(cols[0]), cols[1]);
                });


                foreach (KeyValuePair<DateTime, string> kvp in dic)
                {
                    string compStr = kvp.Key.ToString("yyyyMMdd");
                    if (string.Compare(compDate.ToString("yyyyMMdd"), compStr) == 0)
                    {
                        ret = -1;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                // エラー処理
                MessageBox.Show("曜日計算に失敗しました。ネットワーク環境をご確認下さい。\nErrCode = " + e.HResult.ToString(), "エラーメッセージ");
                // エラー処理後は終了
                this.Close();

            }

            return ret;

        }

        //
        // 売禁データ選択ボタン押下時
        //
        private void savebtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            // デフォルトファイル名
            ofd.FileName = "禁止銘柄一覧.csv";
            // ファイル拡張子指定
            ofd.Filter = "CSVファイル(*.csv)|*.csv";
            // タイトル設定
            ofd.Title = "売禁銘柄ファイルを指定して下さい";
            // DLGクローズ前に現在のフォルダを復元
            ofd.RestoreDirectory = true;


            // ダイアログポップアップ
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                // 指定された場合
                savepath.Text = ofd.FileName.ToString();
                savePathKey = savepath.Text;
            }


        }

        //
        // 設定保存ボタン押下時
        //
        private void save_Click(object sender, EventArgs e)
        {

        }

        //
        //  JPXよりスクレイピング処理
        //    引数：検索する日付
        //    引数：スクレイピングするURL
        //
        private string getJPXScrapingURL(string  targetDate, string url)
        {
            // 指定されたURLに対してのRequestを作成します。
            var req = (HttpWebRequest)WebRequest.Create("https://www.jpx.co.jp/markets/equities/ss-reg/index.html");

            // html取得文字列
            string readhtml = "";
            // 指定したURLに対してReqestを投げてResponseを取得します。
            using (var res = (HttpWebResponse)req.GetResponse())
            using (var resSt = res.GetResponseStream())
            // 取得した文字列をUTF8でエンコードします。
            using (var sr = new StreamReader(resSt, Encoding.UTF8))
            {
                // HTMLを取得する。
                readhtml = sr.ReadToEnd();
                int firstLength = readhtml.IndexOf((targetDate + GET_CSV_NAME));
                if (firstLength > 0)
                {
                    try
                    {
                        // 取得位置自動計算(URL)
                        readhtml = readhtml.Substring((firstLength - 80), 200);
                        firstLength = readhtml.IndexOf("a href=\"");
                        // htmlの先頭文字列の不要部分を削除
                        firstLength = firstLength + 8;
                        readhtml = readhtml.Substring(firstLength);
                        int lastLength = readhtml.IndexOf(GET_CSV_NAME);
                        // htmlの後方文字列調整
                        lastLength = lastLength + GET_CSV_NAME.Length;
                        readhtml = readhtml.Substring(0, lastLength);
                        readhtml = JPX_URL + readhtml;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("データ取得に失敗しました ErrorCode = " + e.ToString(), "エラーメッセージ");
                        this.Close();
                    }

                }
                else
                {
                    // 無い場合は全営業日を指定する
                    if (firstLength == -1)
                    {
                        // 前営業日を指定する
                        readhtml = "-1";
                    }
                    else
                    {
                        MessageBox.Show("データ取得に失敗しました ErrcoDe = -1", "エラーメッセージ");
                        readhtml = "0";
                    }
                }
            }
            return readhtml;

        }


        //
        // 設定読み込み時
        //
        private void RegRead()
        {
            // レジストリ読込み

            // Key作成
            Microsoft.Win32.RegistryKey regKey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(APP_REG_KEY, false);

            if(regKey == null) return;

            lastUpdateKey = (string)regKey.GetValue(APP_REG_SUBKEY_UPDATE_KEY);
            savePathKey = (string)regKey.GetValue(APP_REG_SUBKEY_SAVEPATH_KEY);
//            powerAutoKey = (string)regKey.GetValue(APP_REG_SUBKEY_POWER_AUTO_KEY);

            if (regKey.GetValue(APP_REG_SUBKEY_STOPCOUNT_KEY) != null) {
                stopCount = (int)regKey.GetValue(APP_REG_SUBKEY_STOPCOUNT_KEY);
            }
            
        }

        //
        // 禁止銘柄の停止日数(KeyPress)
        //
        private void stopDayCountKey(object sender, KeyPressEventArgs e)
        {
            // 入力規制：数値のみに変更する
            if (e.KeyChar == '\b')
            {
                if (stopDateInput.TextLength > 1) {
                    // バックスペースは有効とする
                    return;
                }
            }

            if ((e.KeyChar < '0' || e.KeyChar > '9'))
            {
                // 数値以外が押下時はイベントを破棄する
                e.Handled = true;
            }

        }

        //
        // 禁止銘柄の停止日数(KeyDown)
        //
        private void stopDayCountKeyDown(object sender, KeyEventArgs e)
        {
            // 入力規制：数値のみに変更する
            if (e.KeyCode == Keys.Delete)
            {
                if (stopDateInput.TextLength == 1)
                {
                    // 1文字の場合はDeleteKey無効設定
                    e.Handled = true;
                }
            }


        }

        //
        // 指定したディレクトリ内ファイルの削除
        //
        private void deleteTargetDir(string dirPath)
        {

            DirectoryInfo di = new DirectoryInfo(dirPath);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo subDirectory in di.EnumerateDirectories())
            {
                subDirectory.Delete(true);
            }

        }

        //
        // 既存ファイルの禁止銘柄の読込み
        //
        // 引数：path 読み込むパス
        // 引数：読み込んだリストデータを格納する箱
        //
        private bool readExistingStopList(string path, ref List<ReadCSVFileData> readList)
        {
            if (!System.IO.File.Exists(path))
            {
                return false;
            }

            // 読み込みたいCSVファイルのパスを指定して開く
            StreamReader sr = new StreamReader(path, System.Text.Encoding.GetEncoding("shift_jis"));
            // 末尾まで繰り返す
            while (!sr.EndOfStream)
            {
                // CSVファイルの一行を読み込む
                string line = sr.ReadLine();
                // 読み込んだ一行をカンマ毎に分けて配列に格納する
                string[] values = line.Split(',');

                ReadCSVFileData readData = new ReadCSVFileData();
                readData.code = values[0];
                readData.startDate = values[1];
                readData.endDate = values[2];
                readData.flag = values[3];
                readList.Add(readData);

            }
            sr.Close();

            return true;
        }


        //
        // 売禁止銘柄の更新（ファイル更新）
        //
        // 引数：path 読み込むパス
        // 引数：書き込むリストデータ
        //
        private bool writeExistingStopList(string path,  List<ReadCSVFileData> writeList)
        {

            // バックアップ処理
            baukup(path);

            if (!System.IO.File.Exists(path))
            {
                return false;
            }

            using (StreamWriter sw
                = new StreamWriter(
                    path,
                    false,
                    System.Text.Encoding.Default)
            )
            {
                string writeData;
                foreach (ReadCSVFileData Datas in writeList)
                {
                    // 第二引数は意図的に配列へ変換する。
                    writeData = Datas.code + "," + Datas.startDate + "," + Datas.endDate + "," + Datas.flag;
                     sw.WriteLine(writeData);
                }
                sw.Close();
            }


            return true;
        }

        //
        // 売禁止銘柄の更新（マージ）
        //
        // 引数：path 読み込むパス
        // 引数：書き込むリストデータ
        //
        private bool updateStopList(ref List<ReadCSVFileData> updateList)
        {
            string targetPath = dlDir + "\\" + targetDt.ToString("yyyyMMdd") + GET_CSV_NAME;
            DateTime stDate = targetDt;
            DateTime endDate = targetDt;
            bool updateFlag = false;
            bool existence1 = false;
            bool existence2 = false;

            // 仕掛け翌日に指定されている為、前日にする
            stDate = stDate.AddDays(-1);
            // 設定で何日売禁止にするか設定
            short stopCnt = short.Parse(stopDateInput.Text);
            endDate = stDate.AddDays((short)stopCnt);

            // 読み込みたいCSVファイルのパスを指定して開く
            StreamReader sr = new StreamReader(targetPath, System.Text.Encoding.GetEncoding("shift_jis"));

            // 末尾まで繰り返す
            while (!sr.EndOfStream)
            {
                // CSVファイルの一行を読み込む
                string line = sr.ReadLine();
                // 読み込んだ一行をカンマ毎に分けて配列に格納する
                string[] values = line.Split(',');

                List<string> listData = new List<string>();
                listData.AddRange(values);

                ReadCSVFileData readData = new ReadCSVFileData();
                readData.code = listData[0].Replace("\"", "");  // ダブルフォーテーションを取り除いて格納する
                readData.startDate = stDate.ToString("yyyyMMdd");
                readData.endDate = endDate.ToString("yyyyMMdd");
                readData.flag = "1";
                DateTime startData = DateTime.Parse(stDate.ToString("yyyy/MM/dd"));
                DateTime endtData = DateTime.Parse(endDate.ToString("yyyy/MM/dd"));
                updateFlag = false;
                // データ検索
                int length = updateList.Count;
                for (int i = 0; i < length; i++)
                {
                    ReadCSVFileData beforeData = updateList[i];
                    // 初回は処理無し
                    if (!beforeData.code.Equals("コード"))
                    {
                        string st = formatChangeDate(beforeData.startDate);
                        string et = formatChangeDate(beforeData.endDate);
                        DateTime startReadDate = DateTime.Parse(st);
                        DateTime endReadDate = DateTime.Parse(et);
                        // 銘柄検索
                        if (readData.code.Equals(beforeData.code))
                        {
                            // 日付チェック
                            if (startData == startReadDate && updateFlag == false)
                            {
                                // 更新(終了日は指定された日に更新する。既存データより短くなっても指定通りとする)
                                updateList[i].endDate = readData.endDate;
                                updateFlag = true;
                                break;
                            }
                            else if (startData > startReadDate)
                            {

                                // 開始日付は被るが、終了日付が被らないかチェック
                                if (endtData > endReadDate && updateFlag == false)
                                {
                                    // 登録有無
                                    existence1 = false;
                                    // 以降のデータ確認
                                    for (int j = i + 1; j < length; j++)
                                    {
                                        ReadCSVFileData nextData = updateList[j];
                                        if (beforeData.code.Equals(nextData.code) == true &&
                                            beforeData.startDate.Equals(nextData.startDate) == true &&
                                            beforeData.endDate.Equals(nextData.endDate) == true)
                                        {
                                            // 既に売禁データに登録されてる場合は登録しない
                                            existence1 = true;
                                            break;
                                        }
                                    }
                                    if (updateFlag == false && existence1 == false && updateFlag == false)
                                    {
                                        if (serchData(updateList, readData) == false)
                                        {
                                            // 更新
                                            updateList.Add(readData);
                                            updateFlag = true;
                                        }
                                    }
                                }
                            } else {
                                if (updateFlag == false)
                                {
                                    // 登録有無
                                    existence2 = false;
                                    // 以降のデータ確認
                                    for (int j = i + 1; j < length; j++)
                                    {
                                        ReadCSVFileData nextData = updateList[j];
                                        if (beforeData.code.Equals(nextData.code) == true &&
                                            beforeData.startDate.Equals(nextData.startDate) == true &&
                                            beforeData.endDate.Equals(nextData.endDate) == true)
                                        {
                                            // 既に売禁データに登録されてる場合は登録しない
                                            existence2 = true;
                                            break;
                                        }
                                    }
                                    if (updateFlag == false && existence2 == false)
                                    {
                                        if (serchData(updateList, readData) == false)
                                        {
                                            // 更新
                                            updateList.Add(readData);
                                            updateFlag = true;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                if (updateFlag == false)
                {
                    bool isUpdate = true;
                    // 1件も該当しない場合は売禁銘柄追加
                    for (int j = 1; j < length; j++)
                    {
                        ReadCSVFileData beforeData = updateList[j];
                        if (beforeData.code.Equals(readData.code) == true &&
                                                    beforeData.startDate.Equals(readData.startDate) == true &&
                                                    beforeData.endDate.Equals(readData.endDate) == true)
                        {
                            // 既に売禁データに登録されてる場合は登録しない
                            isUpdate = false;
                            break;

                        }
                    }
                    if (isUpdate == true)
                    {
                        updateList.Add(readData);
                    }
                }

            }
            sr.Close();


            return true;
        }

        //
        // レコード検索
        //
        // 引数 serchList: 検索リスト
        // 引数 target   : 検索対象
        //
        // return : true  レコード有り
        //          false レコード無し
        //
        private bool serchData(List<ReadCSVFileData> serchList, ReadCSVFileData target)
        {
            bool ret = false;
            // データ検索
            int length = serchList.Count;
            for (int i = 0; i < length; i++)
            {
                ReadCSVFileData beforeData = serchList[i];
                if (beforeData.code.Equals(target.code) == true &&
                    beforeData.startDate.Equals(target.startDate) == true)
//                    beforeData.endDate.Equals(target.endDate) == true)
                {
                    ret = true;
                    break;

                }

            }
            return ret;
        }

        //
        // 日付の文字列(yyyyMMdd)をyyyy/MM/ddに変換
        //
        private string formatChangeDate(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            if (value.Length != 8) return value;

            var result = value;

            result = result.Insert(4, "/");
            result = result.Insert(7, "/");
            return result;

        }

        //
        // 画面が終了された時に設定データをレジストリ保存
        //
        private void mainscreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            // レジストリに保存

            // Key作成
            Microsoft.Win32.RegistryKey regKey =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(APP_REG_KEY);

            string update = "";
            if (string.Compare(lastUpdateText.Text, URI_NON_UPADTE) != 0)
            {
                update = lastUpdateText.Text;
             }
            // レジストリに各設定値保存
            regKey.SetValue(APP_REG_SUBKEY_UPDATE_KEY, update);
            regKey.SetValue(APP_REG_SUBKEY_SAVEPATH_KEY, savepath.Text);
            regKey.SetValue(APP_REG_SUBKEY_STOPCOUNT_KEY, int.Parse(stopDateInput.Text));
            regKey.Close();
        }
        /*
        //
        // パソコン電源ON時設定時
        //
        private void powerStartChange(object sender, EventArgs e)
        {
            // 設定がされていない場合の処理
            if(powerOnStartCheck.Checked == true)
            {
                if (savepath.Text.Equals("") == true)
                {
                    beforeEvent = true;
                    powerOnStartCheck.Checked = false;
                    MessageBox.Show("売禁データの格納場所を指定してから設定して下さい。",
                        "警告：パソコン電源ON時の自動更新設定に失敗しました",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;

                }
                //  パソコン電源ON時の自動設定登録

                // Key作成
                Microsoft.Win32.RegistryKey regKey =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(APP_AUTORUN_KEY, true);

                // 自動実行を登録
                regKey.SetValue(Application.ProductName, Application.ExecutablePath + " " + AUTO_EXEC_CMD);;
                regKey.Close();
            }
            else
            {
                if(beforeEvent == true)
                {
                    beforeEvent = false;
                    return;
                }

                // Key作成
                Microsoft.Win32.RegistryKey regKey =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(APP_AUTORUN_KEY, true);

                // 自動実行を削除
                regKey.DeleteValue(Application.ProductName);
                regKey.Close();
            }
        }*/

        //
        // 更新ボタン押下処理
        //
        private void execbtnClick(object sender, EventArgs e)
        {
            dateUpdate();

        }

        //
        // 更新ボタン削除処理
        //
        private void deleteBtnClick(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("ソフトの設定情報を全て削除しますか？",
            "ソフト設定情報削除確認",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

            if(result == DialogResult.Yes)
            {

                //  パソコン電源ON時の自動設定解除
                // レジストリの削除
                // キーを書き込み許可で開く
//                Microsoft.Win32.RegistryKey regkey =
//                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(APP_AUTORUN_KEY, true);
                // 本ソフトの自動起動レジストリ削除
//                regkey.DeleteValue("AutoDataUpdate", false);
//                regkey.Close();

                //  本ソフトの設定情報削除
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(APP_REG_FULL_KEY);

                // 画面UIクリア
//                beforeEvent = true;
//                powerOnStartCheck.Checked = false;
                savepath.Text = "";
                savePathKey = "";
                stopDateInput.Text = "3";
                stopCount = 3;
                lastUpdateText.Text = URI_NON_UPADTE;
                lastUpdateKey = "";

                MessageBox.Show("ソフトの設定情報を全て削除しました",
                    "ソフト設定削除通知",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

            }

        }

        //
        // 更新ファイルバックアップ処理
        //
        // 引数 : path 変更を行うファイル
        private void baukup(string path)
        {
            DateTime lastData = System.IO.File.GetLastWriteTime(path);
            string targetFile = DateTime.Now.ToString("yyyMMdd");

            if (targetFile.Equals(lastData.ToString("yyyyMMdd")) == false)
            {
                File.Copy(path, exedir + "\\backup.csv", true);
            }

        }

        //
        // JPXデータ再読込み処理
        //
        private void dataReadBtn(object sender, EventArgs e)
        {
            readJPXData();

        }

        //
        // JPXデータ読込み処理
        //
        private void readJPXData()
        {
            // 売禁データ取得URL（日々の貸借銘柄データのURL）
            // 参考URL："https://www.jpx.co.jp/markets/equities/ss-reg/co3pgt0000001glm-att/20221111_Stocks_Restricted_Nextday.csv";
            /*
                        // コマンドラインで自動実行有無を判定する
                        string[] cmds = System.Environment.GetCommandLineArgs();
                        if (cmds.Length == 2)
                        {
                            if (cmds[1].Equals(AUTO_EXEC_CMD) == true)
                            {
                                // 自動更新処理
                                autoExecCommand();
                                // 自動実行処理後は終了
                                this.Close();
                            }
                        }
            */
            // 現在日付取得
            DateTime dt = DateTime.Now;
        //@test-debug 祝日
        //dt = dt.AddDays(-10); // 11/3 -> 11/2になる
        // dt = dt.AddDays(-11); // 11/2
        // dt = dt.AddDays(10);  //11/23
        STARTPROC:
            // ダウンロード可能な日を取得        public int endtDate; // 売禁終了日付
            if (getTargetDate(dt.ToString("yyyy/MM/dd"), ref targetDt) == false)
            {
                return;
            }
            string targetDate = targetDt.ToString("yyyyMMdd");
            // JPXより当日もしくは前営業にの売禁止銘柄一覧を取得する。
            string stopListURL = getJPXScrapingURL(targetDate, JPX_STOP_LIST_URL);
            if (stopListURL.Equals("-1") == true)
            {
                // 前営業日を指定しなおす
                dt = dt.AddDays(-1);
                goto STARTPROC;
            }
            else if (stopListURL.Equals("0") == true)
            {
                return;
            }

            accessurlstr.Text = stopListURL;
            accessurl = accessurlstr.Text;

            // 実行フォルダのパス取得
            exedir = System.IO.Directory.GetCurrentDirectory();
            // DL先パス作成
            dlDir = exedir + "\\dl";
            dlPath = dlDir + "\\" + targetDt.ToString("yyyyMMdd") + GET_CSV_NAME;
            // フォルダ作成
            Directory.CreateDirectory(dlDir);
            // ダウンロード先フォルダの不要ファイル削除(存在する場合)
            deleteTargetDir(dlDir);

            // 売禁データ格納場所の初期値設定
            savepath.Text = "";

            // レジストリデータ読込み
            RegRead();

            // 画面へ反映
            if ((lastUpdateKey != null) && string.Compare(lastUpdateKey, "") != 0)
            {
                lastUpdateText.Text = lastUpdateKey;
            }
            if (savePathKey != null)
            {
                savepath.Text = savePathKey;
            }
            if (stopCount != 0)
            {
                stopDateInput.Text = stopCount.ToString();
            }

        }

    }
}
