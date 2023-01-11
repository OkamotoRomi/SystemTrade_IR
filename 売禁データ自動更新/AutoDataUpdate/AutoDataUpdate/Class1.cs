using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDataUpdate
{
    // 同じアセンブリならコール可能な型にしておく(internal)
    internal class ReadCSVFileData
    {
        public string code;           // 銘柄コード
        public string startDate;   // 売禁開始日付
        public string endDate;     // 売禁終了日付
        public string flag;           // フラグ
    }
}
