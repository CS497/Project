using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CS497
{

    /// <summary>
    /// An Object that contains the url that points to the professors bio and the bio with stopwords removed
    /// </summary>
    public class Corpus
    {
        public Corpus(string url)
        {
            this.url = url;
        }

        public string url { get; set; }
        public string document { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] stopwords = "a's,able,about,above,according,accordingly,across,actually,after,afterwards,again,against,ain't,all,allow,allows,almost,alone,along,already,also,although,always,am,among,amongst,an,and,another,any,anybody,anyhow,anyone,anything,anyway,anyways,anywhere,apart,appear,appreciate,appropriate,are,aren't,around,as,aside,ask,asking,associated,at,available,away,awfully,be,became,because,become,becomes,becoming,been,before,beforehand,behind,being,believe,below,beside,besides,best,better,between,beyond,both,brief,but,by,c'mon,c's,came,can,can't,cannot,cant,cause,causes,certain,certainly,changes,clearly,co,com,come,comes,concerning,consequently,consider,considering,contain,containing,contains,corresponding,could,couldn't,course,currently,definitely,described,despite,did,didn't,different,do,does,doesn't,doing,don't,done,down,downwards,during,each,edu,eg,eight,either,else,elsewhere,enough,entirely,especially,et,etc,even,ever,every,everybody,everyone,everything,everywhere,ex,exactly,example,except,far,few,fifth,first,five,followed,following,follows,for,former,formerly,forth,four,from,further,furthermore,get,gets,getting,given,gives,go,goes,going,gone,got,gotten,greetings,had,hadn't,happens,hardly,has,hasn't,have,haven't,having,he,he's,hello,help,hence,her,here,here's,hereafter,hereby,herein,hereupon,hers,herself,hi,him,himself,his,hither,hopefully,how,howbeit,however,i'd,i'll,i'm,i've,ie,if,ignored,immediate,in,inasmuch,inc,indeed,indicate,indicated,indicates,inner,insofar,instead,into,inward,is,isn't,it,it'd,it'll,it's,its,itself,just,keep,keeps,kept,know,known,knows,last,lately,later,latter,latterly,least,less,lest,let,let's,like,liked,likely,little,look,looking,looks,ltd,mainly,many,may,maybe,me,mean,meanwhile,merely,might,more,moreover,most,mostly,much,must,my,myself,name,namely,nd,near,nearly,necessary,need,needs,neither,never,nevertheless,new,next,nine,no,nobody,non,none,noone,nor,normally,not,nothing,novel,now,nowhere,obviously,of,off,often,oh,ok,okay,old,on,once,one,ones,only,onto,or,other,others,otherwise,ought,our,ours,ourselves,out,outside,over,overall,own,particular,particularly,per,perhaps,placed,please,plus,possible,presumably,probably,provides,que,quite,qv,rather,rd,re,really,reasonably,regarding,regardless,regards,relatively,respectively,right,said,same,saw,say,saying,says,second,secondly,see,seeing,seem,seemed,seeming,seems,seen,self,selves,sensible,sent,serious,seriously,seven,several,shall,she,should,shouldn't,since,six,so,some,somebody,somehow,someone,something,sometime,sometimes,somewhat,somewhere,soon,sorry,specified,specify,specifying,still,sub,such,sup,sure,t's,take,taken,tell,tends,th,than,thank,thanks,thanx,that,that's,thats,the,their,theirs,them,themselves,then,thence,there,there's,thereafter,thereby,therefore,therein,theres,thereupon,these,they,they'd,they'll,they're,they've,think,third,this,thorough,thoroughly,those,though,three,through,throughout,thru,thus,to,together,too,took,toward,towards,tried,tries,truly,try,trying,twice,two,un,under,unfortunately,unless,unlikely,until,unto,up,upon,us,use,used,useful,uses,using,usually,value,various,very,via,viz,vs,want,wants,was,wasn't,way,we,we'd,we'll,we're,we've,welcome,well,went,were,weren't,what,what's,whatever,when,whence,whenever,where,where's,whereafter,whereas,whereby,wherein,whereupon,wherever,whether,which,while,whither,who,who's,whoever,whole,whom,whose,why,will,willing,wish,with,within,without,won't,wonder,would,wouldn't,yes,yet,you,you'd,you'll,you're,you've,your,yours,yourself,yourselves,zero".Split(',');
        Dictionary<string, Corpus> Corpus;
        string baseUrl = "http://www.cs.purdue.edu";

        public MainWindow()
        {
            InitializeComponent();
            //LoadDataFromWeb();
            LoadDataFromFile();
        }

        public void LoadDataFromFile()
        {
            string json = File.ReadAllText("data.json");
            Corpus = JsonConvert.DeserializeObject<Dictionary<string, Corpus>>(json);
        }

        public async void LoadDataFromWeb()
        {
            int skipCount = 0;
            int total = 0;
            Corpus = new Dictionary<string, Corpus>();
            HttpClient client = new HttpClient();            
            HtmlDocument doc = new HtmlDocument();                        
            Corpus data;

            // Retrieve and Load Html Document
            string response = await client.GetStringAsync(this.baseUrl + "/people/faculty/index.html");
            doc.LoadHtml(response);
            
            // Loop through each table row with a professor and his information
            foreach (var node in doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div[2]/div[1]/table/tr"))
            {
                // Keep track of how many total professors we found
                total++;
                if (node.SelectNodes("td[1]") != null && node.SelectNodes("td[6]/a") != null)
                {
                    // Since professor has bio, add him to the list to search.
                    data = new Corpus(node.SelectNodes("td[6]/a")[0].Attributes["href"].Value);
                    Corpus.Add(node.SelectNodes("td[1]")[0].InnerText, data);
                }
                else
                {
                    // Keep track of how many professors didn't have links to biographies
                    skipCount++;
                }
            }

            // Now that we have links to the bios for all professors lets loop through them and get the text from their bios and remove stopwords
            foreach (var key in Corpus.Keys)
            {
                // Load their bio page and select the node with their bio using XPATH
                var value = Corpus[key];
                var rep2 = await client.GetStringAsync(this.baseUrl + value.url);
                doc.LoadHtml(rep2);
                var node = doc.DocumentNode.SelectNodes("/html/body/div[2]/div/div[2]/div[1]")[0];
                
                // Get the text without HTML and remove new lines
                string text = node.InnerText;
                text = text.Replace("\n", " ");

                // Remove all punctuation
                text = new string(text.Where(c => !char.IsPunctuation(c)).ToArray());

                // Remove stopwords
                foreach (var word in stopwords)
                {
                    text = text.Replace(" " + word + " ", " ");                        
                }
                text = await Stemmer.stemText(text);
                // Save bio text to the professors data
                value.document = text;

                // The API seems to rate limit us (By 403 Errors) so I put a sleep in which seems to have fixed the issue
                Thread.Sleep(2000);
            }
            
            //MessageBox.Show("Skipped " + skipCount + "/" + total);  // Skip Count
            // Write the json to a file so we don't have to keep redownloading it
            string json = JsonConvert.SerializeObject(Corpus);
            StreamWriter writer = File.CreateText("data.json");
            writer.Write(json);
            writer.Close();
            writer.Dispose();
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, double> rank = new Dictionary<string, double>();
            this.tbResults.Text = "";

            // Get Query and Stem it so it matches the Corpus format
            string query = tbQuery.Text;
            query = await Stemmer.stemText(query);            

            // Compute document ranks
            foreach (var key in Corpus.Keys)
            {
                int freq = Regex.Matches(Corpus[key].document, query).Count;
                double r = Okapi.ComputeWeight(Corpus, key, query, freq, 0);
                rank.Add(key, r);
            }
            
            // Order and print results to screen
            int count = 1;
            foreach (var value in rank.Where( v => v.Value != 0).OrderByDescending(v => v.Value))
            {
                this.tbResults.Text += string.Format("{0}. {1}\t{2}\n", count++, value.Key, value.Value);
            }
        }
    }
}
