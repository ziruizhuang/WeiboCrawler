using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace WeiboCrawler
{
    /// <summary>
    /// 
    /// </summary>
    class WeiboUser
    {
        private ulong _uid=0;
        private ulong _followerId=0;
        private string _fnick="";
        private char _sex='?';

        /// <summary>
        /// User's uid
        /// </summary>
        public ulong UID
        {
            get
            {
                return _uid;
            }
        }

        /// <summary>
        /// User's nickname
        /// </summary>
        public string FNick
        {
            get
            {
                return _fnick;
            }
        }
        
        /// <summary>
        /// User's gender
        /// </summary>
        public char Sex
        {
            get
            {
                return _sex;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="__uid">User's uid</param>
        /// <param name="__fnick">User's nickname</param>
        /// <param name="__sex">User's gender</param>
        /// <param name="__followerId">User's follower's uid</param>
        public WeiboUser(ulong __uid, string __fnick, char __sex, ulong __followerId)
        {
            _uid = __uid;
            _fnick = __fnick;
            _sex = __sex;
            _followerId = __followerId;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="__regexMatchUser">Match found by Regex WeiboWeb._rx_li</param>
        /// <param name="__followerId">follower's uid</param>
        public WeiboUser(Match __regexMatchUser, ulong __followerId)
        {
            _followerId = __followerId;
            try
            {
                _uid = ulong.Parse(Regex.Unescape(__regexMatchUser.Groups["uid"].Value));
                _fnick = Regex.Unescape(__regexMatchUser.Groups["fnick"].Value);
                _sex = Regex.Unescape(__regexMatchUser.Groups["sex"].Value)[0];
            }
            catch
            {

            }
        }
        public void StoreSQLite()
        {
            //throw new Exception("Not Implemented");
            Console.WriteLine(String.Format("Store: {0},{1},{2},{3}",_followerId,_uid,_fnick,_sex));
        }
        public void StoreText()
        {
            FileStream fs = new FileStream("users.log",FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(String.Format("{0},{1}", _followerId, _uid, _fnick, _sex));
            sw.Close();
            fs.Close();

        }
    }
}
