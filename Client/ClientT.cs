using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;       // need to add reference to System.Web
using System.Net;       // need to add reference to System.Net
using System.Net.Http;  // need to add reference to System.Net.Http
using Newtonsoft.Json;  // need to add reference to System.Json
using System.Threading;
using System.Windows;

namespace Client
{
    class ClientT
    {
    private HttpClient client = new HttpClient();
    private HttpRequestMessage message;
    private HttpResponseMessage response = new HttpResponseMessage();
    private string urlBase;
    public string status { get; set; }

    //----< set destination url >------------------------------------------

    public ClientT(string url) { urlBase = url; }

    //----< get list of files available for download >---------------------

    public string[] getAvailableFiles()                     // What are we doing here?
    {
      message = new HttpRequestMessage();
      message.Method = HttpMethod.Get;
      message.RequestUri = new Uri(urlBase);
      Task<HttpResponseMessage> task = client.SendAsync(message);
      HttpResponseMessage response1 = task.Result;
      response = task.Result;
      status = response.ReasonPhrase;
      string[] files = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(response1.Content.ReadAsStringAsync().Result);
      return files;
    }
    //----< open file on server for reading >------------------------------

    int openServerDownLoadFile(string fileName)
    {
      message = new HttpRequestMessage();
      message.Method = HttpMethod.Get;
      string urlActn = "?fileName=" + fileName + "&open=download";
      message.RequestUri = new Uri(urlBase + urlActn);
      Task<HttpResponseMessage> task = client.SendAsync(message);
      HttpResponseMessage response = task.Result;
      status = response.ReasonPhrase;
      return (int)response.StatusCode;
    }
    //----< open file on client for writing >------------------------------

    FileStream openClientDownLoadFile(string fileName)
    {
      string path = "../../DownLoad/";

      FileStream down;
      try
      {
        down = new FileStream(path + fileName, FileMode.OpenOrCreate);
      }
      catch
      {
        return null;
      }
      return down;
    }
    //----< read block from server file and write to client file >---------

    byte[] getFileBlock(FileStream down, int blockSize)
    {
      message = new HttpRequestMessage();
      message.Method = HttpMethod.Get;
      string urlActn = "?blockSize=" + blockSize.ToString();
      message.RequestUri = new Uri(urlBase + urlActn);
      Task<HttpResponseMessage> task = client.SendAsync(message);
      HttpResponseMessage response = task.Result;
      Task<byte[]> taskb = response.Content.ReadAsByteArrayAsync();
      byte[] Block = taskb.Result;
      status = response.ReasonPhrase;
      return Block;
    }
    //----< close FileStream on server and FileStream on client >----------

    void closeServerFile()
    {
      message = new HttpRequestMessage();
      message.Method = HttpMethod.Get;
      string urlActn = "?fileName=dontCare.txt&open=close";
      message.RequestUri = new Uri(urlBase + urlActn);
      Task<HttpResponseMessage> task = client.SendAsync(message);
      HttpResponseMessage response = task.Result;
      status = response.ReasonPhrase;
    }
    //----< open file on server for writing >------------------------------

    int openServerUpLoadFile(string fileName)
    {        
            message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            string urlActn = "?fileName=" + fileName + "&open=upload";
            message.RequestUri = new Uri(urlBase + urlActn);
            Task<HttpResponseMessage> task = client.SendAsync(message);
            HttpResponseMessage response = task.Result;
            status = response.ReasonPhrase;
            return (int)response.StatusCode;                    
    }
    //----< open file on client for Reading >------------------------------

    FileStream openClientUpLoadFile(string fileName)
    {
      //string path = "../../UpLoad/";

      FileStream up;
      try
      {
        up = new FileStream(fileName, FileMode.Open);
      }
      catch
      {
        return null;
      }
      return up;
    }
    //----< post blocks to server >----------------------------------------
    void putBlock(byte[] Block)
    {
      message = new HttpRequestMessage();
      message.Method = HttpMethod.Post;
      message.Content = new ByteArrayContent(Block);
      message.Content.Headers.Add("Content-Type","application/http;msgtype=request");
      string urlActn = "?blockSize=" + Block.Count().ToString();
      message.RequestUri = new Uri(urlBase + urlActn);
      Task<HttpResponseMessage> task = client.SendAsync(message);
      HttpResponseMessage response = task.Result;
      status = response.ReasonPhrase;
    }
    //----< downLoad File >------------------------------------------------
    /*
     *  Open server file for reading
     *  Open client file for writing
     *  Get blocks from server
     *  Write blocks to local file
     *  Close server file
     *  Close client file
     */
    public void downLoadFile(string filename)
    {
      Console.Write("\n  Attempting to download file {0} ", filename);
      Console.Write("\n ------------------------------------------\n");

      FileStream down;
      Console.Write("\n  Sending Get request to open file");
      Console.Write("\n ----------------------------------");
      int status = openServerDownLoadFile(filename);
      Console.Write("\n  Response status = {0}\n", status);
      if (status >= 400)
        return;
      down = openClientDownLoadFile(filename);

      Console.Write("\n  Sending Get requests for block from file");
      Console.Write("\n ------------------------------------------");
      while (true)
      {
        int blockSize = 512;
        byte[] Block = getFileBlock(down, blockSize);
        Console.Write("\n  Response status = {0}", status);
        Console.Write("\n  received block of size {0} bytes\n", Block.Length);
        if (Block.Length == 0 || blockSize <= 0)
          break;
        down.Write(Block, 0, Block.Length);
        if (Block.Length < blockSize)    // last block
          break;
      }

      Console.Write("\n  Sending Get request to close file");
      Console.Write("\n -----------------------------------");

      closeServerFile();
      Console.Write("\n  Response status = {0}\n", status);
      down.Close();
    }
    //----< upLoad File >--------------------------------------------------
    /*
     *  Open server file for writing
     *  Open client file for reading
     *  Read blocks from local file
     *  Send blocks to server
     *  Close server file
     *  Close client file
     */
    public void upLoadFile(string filename)
    {
        //try
        //{
            Console.Write("\n  Attempting to upload file {0}", filename);
            Console.Write("\n --------------------------------------\n");

            Console.Write("\n  Sending get request to open file");
            Console.Write("\n ----------------------------------");

            string file = System.IO.Path.GetFileName(filename);
            openServerUpLoadFile(file);
            Console.Write("\n  Response status = {0}\n", status);
            FileStream up = openClientUpLoadFile(filename);

            Console.Write("\n  Sending Post requests to send blocks:");
            Console.Write("\n ---------------------------------------");

            const int upBlockSize = 512;
            byte[] upBlock = new byte[upBlockSize];
            int bytesRead = upBlockSize;
            while (bytesRead == upBlockSize)
            {
                bytesRead = up.Read(upBlock, 0, upBlockSize);
                if (bytesRead < upBlockSize)
                {
                    byte[] temp = new byte[bytesRead];
                    for (int i = 0; i < bytesRead; ++i)
                        temp[i] = upBlock[i];
                    upBlock = temp;
                }
                Console.Write("\n  sending block of size {0}", upBlock.Count());
                putBlock(upBlock);
                Console.Write("\n  status = {0}\n", status);
            }

            Console.Write("\n  Sending Get request to close file");
            Console.Write("\n -----------------------------------");

            closeServerFile();
            Console.Write("\n  Response status = {0}\n", status);
            up.Close();
       // }
        //catch(Exception ex)
        //{
            
        //}
    }
    }
}
