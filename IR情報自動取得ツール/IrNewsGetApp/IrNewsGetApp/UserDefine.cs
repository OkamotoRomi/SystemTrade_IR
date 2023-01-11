using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrNewsGetApp
{
    // 同じアセンブリならコール可能な型にしておく(internal)
    class IRDataRecord
    {
        public string code = null;           // 銘柄コード
        public string codeName = null;       // 銘柄名
        public string irNews = null;         // IR情報
        public string kbn = null;            // 対象有無
        public string url = null;            // IRのURL
        public string remarks = null;        // 備考
    }
}
