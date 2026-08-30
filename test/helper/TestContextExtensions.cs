using Lestaly;

namespace NebriaChip.Ip.Win.Tests;

public static class TestContextExtensions
{
    public static DirectoryInfo SolutionDir(this TestContext self)
    {
        return ThisSource.FindAncestor("NebriaChip.Ip.Win") ?? throw new Exception("ソリューションディレクトリが見つからない");
    }

    public static DirectoryInfo LibProjectDir(this TestContext self)
    {
        return self.SolutionDir().RelativeDirectory("src").ThrowIfNotExists(d => new Exception("ライブラリプロジェクトディレクトリが見つからない"));
    }

    public static DirectoryInfo TestProjectDir(this TestContext self)
    {
        return self.SolutionDir().RelativeDirectory("test").ThrowIfNotExists(d => new Exception("テストプロジェクトディレクトリが見つからない"));
    }

    public static DirectoryInfo TestAssetsDir(this TestContext self)
    {
        return self.TestProjectDir().RelativeDirectory("assets").ThrowIfNotExists(d => new Exception("テストアセットディレクトリが見つからない"));
    }

    public static FileInfo TestAssetFile(this TestContext self, string path)
    {
        return self.TestAssetsDir().RelativeFile(path).ThrowIfNotExists(d => new Exception($"テストアセットファイルが見つからない: {d.FullName}"));
    }
}
