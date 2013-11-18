using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CS497
{
    static class Stemmer
    {
        public static async Task<string> stemText(string input)
        {
            string url = "https://www.enclout.com/stemmer/show.json?auth_token=vzgKwHeTQBMGCqsXDv3n&text=";
            HttpClient client = new HttpClient();
            url = url + input.Replace(" ", "%20");
            string ret = await client.GetStringAsync(url);            
            dynamic dyno = JsonConvert.DeserializeObject(ret);
            return dyno.porter_stem;
        }
    }
}
