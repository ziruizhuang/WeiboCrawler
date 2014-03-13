using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeiboCrawler
{
    class WeiboCrawler
    {
        private uint _uid = 0;
        private int _depth = 2;
        private HashSet<uint> _scannedList;
        
        public WeiboCrawler(uint __uid)
        {
            _uid = __uid;
            _depth = 2;
            _scannedList = new HashSet<uint>();
        }
        public WeiboCrawler(uint __uid, int __depth)
        {
            _uid = __uid;
            _depth = __depth;
            _scannedList = new HashSet<uint>();
        }
        private void _findUsers(int __depth, ref List<uint> __current)
        {
            throw new Exception("Not implemented");

            if (__depth > _depth)
            {
                return;
            }
            else
            {
                List<uint> next = new List<uint>();             
                foreach (uint uid in __current)
                {
                    //TODO: BFS
                    _findUser(uid, ref next);
                    __current.Remove(uid);
                }
                _findUsers(__depth + 1, ref next);
            }
        }
        private void _findUser(uint __source, ref List<uint> __next)
        {
            throw new Exception("Not Implemented");
            if (_scannedList.Contains(__source))
            {
                return;
            }
            else
            {
                //TODO: HTTP Events
                string webDocHtml = "";
                foreach (Match li in WeiboWeb.ParseHtml(ref webDocHtml))
                {
                    WeiboUser u = new WeiboUser(li,__source);
                    u.StoreSQLite();
                    __next.Add(u.UID);
                    _scannedList.Add(u.UID);
                }
            }
        }
        
        private void FindUsers()
        {
            throw new Exception("Not implemented");
            List<uint> current = new List<uint>();
            current.Add(_uid);
            _findUsers(0, ref current);
        }
    }
}
