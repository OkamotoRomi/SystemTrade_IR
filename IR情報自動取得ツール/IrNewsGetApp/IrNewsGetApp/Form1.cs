using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IrNewsGetApp
{
    public partial class Form1 : Form
    {
        // 実行ファイルが格納されているパス
        private string exedir = null;
        // 読込みデータファイルのパス
        private string readFilePath = null;


        public Form1()
        {
            InitializeComponent();
        }

        //
        // メイン画面起動時
        //
        private void scrennLoad(object sender, EventArgs e)
        {
            // カラム数を指定
            //            dataGridView.ColumnCount = 5;

        }

        //
        // IR取得ボタン押下時
        //
        private async void irBtl(object sender, EventArgs e)
        {
            getBtn.Enabled = false;

            // 実行フォルダのパス取得
            exedir = System.IO.Directory.GetCurrentDirectory();
            //TODO@暫定パス作成
            readFilePath = exedir + "\\2022_1127_賃借銘柄.csv";
            readFileNameText.Text = readFilePath;

            // 対象銘柄ファイル読み込み
            List<IRDataRecord> lists = new List<IRDataRecord>();
            if (readFileList(readFilePath, ref lists) == false)
            {
                // 処理失敗
                MessageBox.Show("対象銘柄ファイルの読み込みに失敗しました。",
                    "警告",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                getBtn.Enabled = true;
                return;
            }

            if (urlAnalysus(ref lists) == false)
            {
                // 処理失敗
                MessageBox.Show("銘柄URLのスクレピングに失敗しました。",
                    "警告",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                getBtn.Enabled = true;
                return;

            }

            // データグリッドに反映
            DataTable table = new DataTable("DataRecord");

            table.Columns.Add("項番");
            table.Columns.Add("銘柄コード");
            table.Columns.Add("銘柄名");
            //            table.Columns.Add("情報");
            table.Columns.Add("URL");
            table.Columns.Add("備考");

//            table.Rows[0].Cells[m]

            //            table.auto
            //            dataGridView.Columns.auto
            //            dataGridView.Columns[0].width = 30;
            int j = 0;

            for (int i = 0; i < lists.Count; i++)
            {
                //                if (lists[i].kbn.Equals("1") == false &&
                //                    lists[i].irNews.Equals("無し") == false)
                if (lists[i].kbn.Equals("1") == false)
                {
                    if (lists[i].irNews.Equals("無し") == false) {
                        table.Rows.Add(
                        j.ToString(),
                        lists[i].code,
                        lists[i].codeName,
                        //                        lists[i].irNews,
                        lists[i].url,
                        lists[i].remarks);
                        dataGridView.DataSource = table;
//                        if (lists[i].irNews.Equals("取得失敗") == true ||
                        if(lists[i].remarks.Equals("403:Not Found Error") == true)
//                        if (lists[i].remarks.IndexOf("403") > 0)
                        {
                            for (int m = 0; m < 5; m++)
                            {
                                dataGridView.Rows[j].Cells[m].Style.BackColor = Color.Yellow;
                            }
                        }
                        j++;
                    }
                }
            }

            // 最低でもヘッダは表示
            dataGridView.DataSource = table;
            dataGridView.Columns[0].Width = 35;
            dataGridView.Columns[1].Width = 40;
            dataGridView.Columns[2].Width = 130;
            dataGridView.Columns[3].Width = 200;
            dataGridView.Columns[4].Width = 480;
            dataGridView.Columns[4].Width = 150;


            getBtn.Enabled = true;

        }

        //
        // URL解析処理
        //
        // param  : 銘柄リスト
        // return : true:当日IRあり判定
        private bool urlAnalysus(ref List<IRDataRecord> readList)
        {
            bool ret = true;
           string msg = "";

            foreach (IRDataRecord Datas in readList)
            {
                if (Datas.kbn.Equals("1") == false)
                {
                    // スクレイピング対象銘柄に対してIR調査
                    if (Datas.url.Equals("") == false)
                    {
                        // 第二引数は意図的に配列へ変換する。
                        //                        writeData = Datas.code + "," + Datas.startDate + "," + Datas.endDate + "," + Datas.flag;
                         string ir = isIRCheck(Datas.url);
                        if (ir.Equals("") == true)
                        {
                            Datas.kbn = "1";
                            Datas.irNews = "無し";
                        }
                        else
                        {
                            if (ir.Equals("-1") == true)
                            {
                                Datas.irNews = "取得失敗";
                                Datas.remarks = "IR取得失敗";
                                msg = msg + "\n" + Datas.code + " の取得に失敗しました";
                            }
                            else
                            {
                                {
                                    if (ir.Equals("-2146233079") == true)
                                    {
                                        Datas.remarks = "403:Not Found Error";
                                        Datas.irNews = "取得失敗";
                                    }
                                    else
                                    {

                                        Datas.irNews = ir;
                                    }
                                }
                            }

                        } 
                    }

                    codeDatailLabel.Text = Datas.code + " のURL解析中・・・" + msg;
                    codeDatailLabel.Update();
                }

                codeDatailLabel.Text = "各銘柄のURL解析完了" + msg;
            } 
            
                return ret;
        }
        
        //
        // IR判定
        //
        // return  "" : IRなし
        //         ""以外 : IRあり
        //
        private string isIRCheck(String url)
        {
            string ret = "";

            if (url.Equals("") == true)
            {
                // URLなしなら処理無し
                return ret;
            }
            try
            {
                //            } catch (WebException we)
                //                {
                // エラー処理後は終了
                //                ret = we.HResult.ToString();

                //                var client = new System.Net.WebClient();
                //                byte[] buffer = client.DownloadData(_path);
                //                string str = Encoding.GetEncoding("shift_jis").GetString(buffer);

                WebClient wc = new WebClient();
                //                wc.Headers.add("User-Agent", "hoge");
                wc.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.0.0 Safari/537.36");
                string htmlData = wc.DownloadString(url);

                DateTime targetData = DateTime.Now;
                getDate(ref targetData);
                string targetDataStr_yyyy = targetData.ToString("yyyyd");
                string targetDataStr_mm = targetData.ToString("MM");
                string targetDataStr_dd = targetData.ToString("dd");
                string targetDataStr = targetData.ToString("yyyy/MM/dd");

                int serachRet = htmlData.IndexOf(targetDataStr);
                if (serachRet > 0)
                {
                    ret = htmlData;
                } else
                {
                    targetDataStr = targetData.ToString("yyyy.MM.dd");
                    serachRet = htmlData.IndexOf(targetDataStr);
                    if (serachRet > 0)
                    {
                        ret = htmlData;
                    } else
                    {
                        targetDataStr = targetData.ToString("yyyy年MM月dd");
                        serachRet = htmlData.IndexOf(targetDataStr);
                        if (serachRet > 0)
                        {
                            ret = htmlData;
                        } else
                        {
                            targetDataStr = targetData.ToString("yyyy-MM-dd");
                            serachRet = htmlData.IndexOf(targetDataStr);
                            if (serachRet > 0)
                            {
                            }
                            else
                            {
                                ret = "";
                            }
                        }
                    }

                }
            }
            catch (System.Net.WebException we)
            {
                //HTTPプロトコルエラーかどうか調べる
                if (we.Status == System.Net.WebExceptionStatus.ProtocolError)
                {
                    //HttpWebResponseを取得
                    System.Net.HttpWebResponse errres =
                        (System.Net.HttpWebResponse)we.Response;
                    //応答したURIを表示する
                    //                    Console.WriteLine(errres.ResponseUri);
                    //応答ステータスコードを表示する
                    //                    Console.WriteLine("{0}:{1}",
                    //                        errres.StatusCode, errres.StatusDescription);

                    ret = we.HResult.ToString();
                }
            }
            catch (Exception e)
            {
                // エラー処理後は終了
                ret = "-1";
            }
        
            return ret;
        }
        

        //
        // 対象銘柄取得＆URL取得のためのファイルの読込み
        //
        // 引数：path 読み込むパス
        // 引数：読み込んだリストデータを格納する箱
        //
            private bool readFileList(string path, ref List<IRDataRecord> readList)
            {
                if (!System.IO.File.Exists(path))
                {
                    return false;
                }

                try
                {
                    // 読み込みたいCSVファイルのパスを指定して開く
                    StreamReader sr = new StreamReader(path, System.Text.Encoding.GetEncoding("shift_jis"));
                    // 先頭のヘッダ削除
                    sr.ReadLine();
                    // 末尾まで繰り返す
                    while (!sr.EndOfStream)
                    {
                        // CSVファイルの一行を読み込む
                        string line = sr.ReadLine();
                        // 読み込んだ一行をカンマ毎に分けて配列に格納する
                        string[] values = line.Split(',');

                        IRDataRecord readData = new IRDataRecord();
                        readData.code = values[0];         // 銘柄コード
                        readData.codeName = values[1];     // 銘柄名
                        readData.irNews = "無し";          // IR情報(未取得)
                        readData.kbn = values[2];          // 区分(対象銘柄有無：1が無効)
                        readData.url = values[3];          // IRのURL
                        readData.remarks = values[4];          // IRのURL
                        readList.Add(readData);
                    }
                    sr.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("データ取得に失敗しました ErrorCode = " + e.ToString(), "エラーメッセージ");
                    this.Close();
                }

                return true;

            }

            //
            // ターゲット日付取得処理
            //
            private bool getDate(ref DateTime dt)
            {
                bool ret = true;

                // 現在日付取得
                dt = DateTime.Now;

                // ダウンロード可能な日を取得        public int endtDate; // 売禁終了日付
                if (getTargetDate(dt.ToString("yyyy/MM/dd"), ref dt) == false)
                {
                    ret = false;
                }

                return ret;
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
                string _path = @"https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv";

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
                switch (dayWeek)
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

            private void cell_doubleclick(object sender, DataGridViewCellEventArgs e)
            {
                //            string s1 = $"ダブルクリックされた位置 {e.RowIndex}列目 {e.ColumnIndex}行目";

                // 列ヘッダー
                if (e.RowIndex == -1)
                {
                    MessageBox.Show(Environment.NewLine + "列ヘッダーです。", "情報",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 行ヘッダー
                if (e.ColumnIndex == -1)
                {
                    MessageBox.Show(Environment.NewLine + "行ヘッダーです。", "情報",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (e.ColumnIndex != 3)
                {
                    return;
                }

                string url = $"{dataGridView[e.ColumnIndex, e.RowIndex].Value}";

                if (url.StartsWith("http"))
                {
                    System.Diagnostics.Process.Start(url);
                }
            }

        }
    }

