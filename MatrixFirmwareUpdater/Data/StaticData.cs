﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MatrixFirmwareUpdater.Data
{
    public class GithubRelease
    {
        public class Author
        {
            public string login { get; set; }
            public int id { get; set; }
            public string node_id { get; set; }
            public string avatar_url { get; set; }
            public string gravatar_id { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string followers_url { get; set; }
            public string following_url { get; set; }
            public string gists_url { get; set; }
            public string starred_url { get; set; }
            public string subscriptions_url { get; set; }
            public string organizations_url { get; set; }
            public string repos_url { get; set; }
            public string events_url { get; set; }
            public string received_events_url { get; set; }
            public string type { get; set; }
            public bool site_admin { get; set; }
        }

        public class Uploader
        {
            public string login { get; set; }
            public int id { get; set; }
            public string node_id { get; set; }
            public string avatar_url { get; set; }
            public string gravatar_id { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string followers_url { get; set; }
            public string following_url { get; set; }
            public string gists_url { get; set; }
            public string starred_url { get; set; }
            public string subscriptions_url { get; set; }
            public string organizations_url { get; set; }
            public string repos_url { get; set; }
            public string events_url { get; set; }
            public string received_events_url { get; set; }
            public string type { get; set; }
            public bool site_admin { get; set; }
        }

        public class Asset
        {
            public string url { get; set; }
            public int id { get; set; }
            public string node_id { get; set; }
            public string name { get; set; }
            public object label { get; set; }
            public Uploader uploader { get; set; }
            public string content_type { get; set; }
            public string state { get; set; }
            public int size { get; set; }
            public int download_count { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public string browser_download_url { get; set; }
        }
        public string url { get; set; }
        public string assets_url { get; set; }
        public string upload_url { get; set; }
        public string html_url { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string tag_name { get; set; }
        public string target_commitish { get; set; }
        public string name { get; set; }
        public bool draft { get; set; }
        public Author author { get; set; }
        public bool prerelease { get; set; }
        public DateTime created_at { get; set; }
        public DateTime published_at { get; set; }
        public List<Asset> assets { get; set; }
        public string tarball_url { get; set; }
        public string zipball_url { get; set; }
        public string body { get; set; }
    }
    public class MatrixFWMeta
    {
        public string version { get; set; }
        public string build_type { get; set; }
        public DateTime publish_time { get; set; }
        public IList<string> supported_device { get; set; }
        public string patchnote_en { get; set; }
        public string patchnote_zh_cn { get; set; }
        public string file_URL { get; set; }

    }

    public class StaticData
    {
        public class MatrixInfo
        {
            public MatrixInfo(string name, string FW_Version, string Serial_number, string status)
            {
                this.Name = name;
                this.FW_Version = FW_Version;
                this.Serial_number = FW_Version;
                this.Status = status;
            }
            public string Name { get; set; }
            public string FW_Version { get; set; }
            public string Serial_number { get; set; }
            public string Status { get; set; }
        }

        public static MatrixInfo matrix { get; set; }

        public static MatrixFWMeta matrixFW;

        public bool sysex_replied;

        public class MatrixFWMeta
        {
            public string Version { get; set; }
            public string Build_type { get; set; }
            public DateTime Publish_time { get; set; }
            public IList<string> Supported_devices { get; set; }
            public string Patchnote_en { get; set; }
            public string Patchnote_zh_CN { get; set; }
            public string File_URL { get; set; }
        }

        public static void SetMatrixFWMetaData(TextBlock tbVersion, TextBlock tbPatchnote)
        {
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                //要更新的UI代码
                try
                {
                    tbVersion.Text = matrixFW.Version;
                    tbPatchnote.Text = matrixFW.Patchnote_zh_CN;
                }
                catch (Exception)
                {

                }
            });
        }
    }
}