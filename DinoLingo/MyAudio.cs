using System;
using Plugin.SimpleAudioPlayer;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace DinoLingo
{
    public class MyAudio
    {
        public ISimpleAudioPlayer playerBackground, playerVictory, playerCorrect, playerWrong, sayWord;

		public MyAudio()
        {
            playerBackground = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            playerBackground.Load(GetStreamFromFile("toon.mp3"));
            playerBackground.Loop = true;

            sayWord = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            Debug.WriteLine("sayWord -> created");
            playerVictory = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            playerVictory.Load(GetStreamFromFile("Crowd.mp3"));
            playerVictory.Volume = 0.3;

            playerCorrect = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            playerCorrect.Load(GetStreamFromFile("CORRECT.mp3"));

            playerWrong = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            playerWrong.Load(GetStreamFromFile("WRONG.mp3"));
        }


        Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("DinoLingo.Audio." + filename);
            return stream;
        }

        public async void SayWord(string key, string language)
        {
            Debug.WriteLine("MyAudio -> SayWord -> key = " + key);
            SayWord(key + "_" + language + ".mp3");
        }

        public void SayWordFromRes (string resName) {
            Stream stream = GetStreamFromFile(resName);
            Debug.WriteLine("stream == null ? :" + (stream == null));
            sayWord.Load(stream);
            Debug.WriteLine("sayWord == null ? :" + (sayWord == null));
            Debug.WriteLine("sayWord.IsPlaying :" + sayWord.IsPlaying);
            Debug.WriteLine("try to play: " + resName);  

            sayWord.Play();
        }

        public async void SayWord(string path)
        {
            Debug.WriteLine("Try to say word: + " + path);

            if (sayWord.IsPlaying) sayWord.Stop();

            if (await PCLHelper.IsFileExistAsync(path))
            {
                Stream strm = await PCLHelper.GetStreamForFile(path);
                sayWord.Load(strm);
                sayWord.Play();
            }
            else
            {
                Debug.WriteLine("file = " + path + " does not exist, CANNOT PLAY!");
            }
        }
    }


}
