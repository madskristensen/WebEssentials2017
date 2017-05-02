using System;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.ExtensionManager;

namespace WebEssentials
{
    public class GalleryEntry : GalleryOnlineExtension, IRepositoryEntry
    {
        public override string VsixID { get; set; }
        public override string VsixVersion { get; set; }
        public override DateTime LastModified { get; set; }
        public override string Author { get; set; }
        public override string Icon { get; set; }
        public override string MoreInfoURL { get; set; }
        public override string ReferralUrl { get; set; }
        public override string ReportAbuseUrl { get; set; }
        public override double Rating { get; set; }
        public override int RatingsCount { get; set; }
        public override int DownloadCount { get; set; }
        public override string Name { get; set; }
        public override string Id { get; set; }
        public override string Description { get; set; }

        public override float Priority => throw new NotImplementedException();

        public override BitmapSource MediumThumbnailImage => throw new NotImplementedException();

        public override BitmapSource SmallThumbnailImage => throw new NotImplementedException();

        public override bool IsSelected { get; set; }
        public override string DownloadUrl { get; set; }
        public override string DownloadUpdateUrl { get; set; }
        public override string VsixReferences { get; set; }
        public override string Type { get; set; }
        public override string ProjectTypeFriendly { get; set; }
        public override bool SupportsCodeSeparation { get; set; }
        public override bool SupportsMasterPage { get; set; }
        public override string OnlinePreviewImage { get; set; }
        public override string DefaultName { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}