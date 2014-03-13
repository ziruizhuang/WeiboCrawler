using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeiboCrawler
{
    class WeiboUser
    {
        private uint _uid;
        private uint _followerId;
        private string _fnick;
        private char _sex;
        public uint UID
        {
            get
            {
                return _uid;
            }
        }
        public string FNick
        {
            get
            {
                return _fnick;
            }
        }
        public char Sex
        {
            get
            {
                return _sex;
            }
        }
        public WeiboUser(uint __uid, string __fnick, char __sex, uint __followerId)
        {
            _uid = __uid;
            _fnick = __fnick;
            _sex = __sex;
            _followerId = __followerId;
        }
        public WeiboUser(Match __regexMatchUser, uint __followerId)
        {
            _followerId = __followerId;
            try
            {
                _uid = uint.Parse(Regex.Unescape(__regexMatchUser.Groups["uid"].Value));
                _fnick = Regex.Unescape(__regexMatchUser.Groups["fnick"].Value);
                _sex = Regex.Unescape(__regexMatchUser.Groups["sex"].Value)[0];
            }
            catch
            {

            }
        }
        public void StoreSQLite()
        {
            throw new Exception("Not Implemented");
        }
    }
}
