using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAPI.Hateoas
{
    public class Hateoas
    {
        public string url;
        public string protocol = "https://";
        public List<Link> actions = new List<Link>();


        public Hateoas(string url)
        {
            this.url = url;
        }

        public Hateoas(string url, string protocol)
        {
            this.url = url;
            this.protocol = protocol;
        }

        public void AddAction(string rel, string method)
        {
            //https://(protocolo)localhost:5001/api/v1/Produtos(url)
            actions.Add(new Link(this.protocol + this.url, rel, method));
        }

        public Link[] GetActions(string sufix)
        {
            Link[] tempLinks = new Link[actions.Count];

            for(int i = 0; i < tempLinks.Length; i++)
            {
                tempLinks[i] = new Link(actions[i].href, actions[i].rel, actions[i].method);
            }



            //Montagem do Link
            foreach (var link in tempLinks)
            {
                //https://(protocolo)localhost:5001/api/v1/Produtos(url)/id
                link.href = link.href + "" + sufix;
            }
            return tempLinks;
        }

    }
}
