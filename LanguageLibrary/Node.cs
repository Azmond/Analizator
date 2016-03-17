using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageLibrary
{
    public class Node : LocationEntity
    {
        public string result;
        public bool ErrMasg;
        public string head;

        public Node()
        {
            head = "";
            result = "";
            ErrMasg = false;
        }

    }

    public class ParseLanguageNode : Node
    {

    }


    public class ParseDefinitionsNode : Node
    {

    }

    public class ParseEndingNode : Node
    {

    }

    public class ParseDefinitionNode : Node
    {

    }

    public class ParseComplexNode : Node
    {

    }

    public class ParseFloatNode : Node
    {

    }

    public class ParseRightPartNode : Node
    {

    }

    public class ParseBlokNode : Node
    {

    }

    public class ParseBlok1Node : Node
    {

    }

    public class ParseBlok2Node : Node
    {

    }

    public class ParseBlok3Node : Node
    {

    }

}
