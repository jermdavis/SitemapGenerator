# Sitemap Generator

![Build Status](https://clerkswell.visualstudio.com/_apis/public/build/definitions/bc27bb6a-7ada-4ec7-a59b-e6695807072a/2/badge)

A simple example of generating an XML sitemap file after a Sitecore publish operation. 

Written as an example project for some blog posts - not intended as production code.

**NB:** This solution references private NuGet packages. To build and run tests you
will need to provide your own alternative source for the Hedgehog TDS binaries package,
and you will need to produce your own version of the Sitecore.FakeDB.Licensing package.
See:

* https://jermdavis.wordpress.com/2016/08/08/sitecore-builds-with-visual-studio-online-part-1-private-nuget-feeds/
* https://jermdavis.wordpress.com/2016/08/22/sitecore-builds-with-visual-studio-online-part-2-building-code-and-running-tests/

for ideas about what you can do here.

## Installation

Install the `.update` package supplied using your preferred method, and then publish:

* `/Templates/SitemapGenerator`
* `/System/Modules/SitemapGenerator`

If you wish to be able to set the update frequency and priority of individual items in
the sitemap file, you will need to add the sitemap properties template to your content
templates. Ensure each content template you plan to add to your sitemap inherits from
`/Templates/SitemapGenerator/SitemapProperties`. 

## Configuration

1. Go to `/System/Modules/SitemapGenerator/SitemapDefinitions` and insert a new 
   definition item.
2. Fill in its fields:
   * `FilenameToGenerate`: Specify the name of the file you want the generator to 
     write. It will be stored in the root of your Sitecore Website folder. 
   * `RootItem`: Choose the content item which is the root for this Sitemap. Probably
     the homepage of the site you are generating the sitemap for.
   * `SourceDatabase`: Which Sitecore content database should the root item be read
     from. If you are working with multiple publishing targets, add the names of your
     target databases to `/System/Modules/SitemapGenerator/Databases` in order to
     be able to pick them here.
   * `TemplatesToInclude`: If you wish to filter the content items under the root and
     write only those which match certain templates, then select the templates to include
     here. If this field is empty, all items will be included.
   * `LanguagesToInclude`: If you wish only certain language versions to be included
     in the output, then select them here. The options come from the standard Sitecore
     language items. If this field is empty, all language versions will be included.

Note that:
* If you add multiple definitions, multiple output files will be generated.

## Usage

The module will update the sitemap file after each publish operation. No other actions
are required to keep it in sync with the content.

