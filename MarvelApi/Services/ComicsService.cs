﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using MarvelApi.Models;
using ServiceStack;

namespace MarvelApi.Services
{
    [Route("/Comics")]
    public class ComicRequest
    {
        public string Title { get; set; }
    }
    
    [Route("/Comics/{ComicId}/Characters")]
    public class ComicCharactersRequest
    {
        public int ComicId { get; set; }
    }

    public class ComicsService : Service
    {
        public ComicDataWrapper Get(ComicRequest request)
        {
            var ts = Guid.NewGuid();
            var hash = GetMd5Hash(MD5.Create(), ts + ConfigurationManager.AppSettings["marvelPrivateKey"] + ConfigurationManager.AppSettings["marvelPublicKey"]);
            var url = "http://gateway.marvel.com/v1/public/comics"
                .AddQueryParam("ts", ts)
                .AddQueryParam("apikey", "de057f1f51e36402aeeafea0fd5a5936")
                .AddQueryParam("hash", hash)
                .AddQueryParam("title", "Avengers: The Initiative");


            return url.GetJsonFromUrl().FromJson<ComicDataWrapper>();
        }

        public CharacterDataWrapper Get(ComicCharactersRequest request)
        {
            var ts = Guid.NewGuid();
            var hash = GetMd5Hash(MD5.Create(), ts + ConfigurationManager.AppSettings["marvelPrivateKey"] + ConfigurationManager.AppSettings["marvelPublicKey"]);
            var url = "http://gateway.marvel.com/v1/public/comics/" + request.ComicId + "/characters"
                .AddQueryParam("ts", ts)
                .AddQueryParam("apikey", "de057f1f51e36402aeeafea0fd5a5936")
                .AddQueryParam("hash", hash);

            var resp = url.GetJsonFromUrl().FromJson<CharacterDataWrapper>();

            return resp;
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
    }
}