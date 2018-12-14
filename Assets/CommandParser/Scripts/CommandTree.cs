using System.Collections.Generic;

namespace Command
{

    namespace Actions
    {

        public interface INode
        {
            void Accept(CommandVerify v);
        }

        public class Sequence : INode
        {
            public List<INode> data = new List<INode>();
            public void Accept(CommandVerify v)
            {
                v.Visit(this);
            }
        };

        public class SameTime : INode
        {
            public List<INode> data = new List<INode>();
            public void Accept(CommandVerify v)
            {
                v.Visit(this);
            }
            public SameTime(List<INode> data_)
            {
                data = data_;
            }

        };

        public class Press : INode
        {
            public string button;
            public void Accept(CommandVerify v)
            {
                v.Visit(this);
            }
            public Press(string button_)
            {
                button = button_;
            }
        };

        public class Hold : INode
        {
            public string button;
            public int time;
            public void Accept(CommandVerify v)
            {
                v.Visit(this);
            }
        };

        public class Release : INode
        {
            public string button;
            public int time;
            public void Accept(CommandVerify v)
            {
                v.Visit(this);
            }
        };

        public enum Vertical
        {
            UP,
            DOWN
        };

        public enum Horizontal
        {
            FRONT,
            BACK
        };

        public class Press_Dir : INode
        {
            public Vertical? v;
            public Horizontal? h;
            public bool only;
            public void Accept(CommandVerify v)
            {
                v.Visit(this);
            }
            public Press_Dir(Vertical? v_, Horizontal? h_, bool only_ = false)
            {
                v = v_;
                h = h_;
                only = only_;
            }
        };

        public class Hold_Dir : INode
        {
            public Vertical? v;
            public Horizontal? h;
            public int time;
            public void Accept(CommandVerify v)
            {
                v.Visit(this);
            }
            public Hold_Dir(Vertical? v_, Horizontal? h_, int time_ = 1)
            {
                v = v_;
                h = h_;
                time = time_;
            }

        };

        public class Release_Dir : INode
        {
            public Vertical? v;
            public Horizontal? h;
            public int time;
            public void Accept(CommandVerify v)
            {
                v.Visit(this);
            }
            public Release_Dir(Vertical? v_, Horizontal? h_, int time_ = 1)
            {
                v = v_;
                h = h_;
                time = time_;
            }
        };


    }

}
