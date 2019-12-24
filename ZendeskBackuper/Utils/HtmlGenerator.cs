using System.Collections.Generic;
using ZendeskBackuper.Backuper.Models;
using System.Configuration;

namespace ZendeskBackuper.Backuper.Utils
{
    class HtmlGenerator
    {
        public static string GetHtmlForArticlesIndex(List<Article> articles)
        {
            string result = "";

            foreach (Article article in articles)
            {
                result += string.Format("<li><a href=\"./{0}/{1}\">{2}</a></li>", article.Id, Tools.AppConfiguration.IndexFileName, article.Name);
            }

            return string.Format("<ul>{0}</ul>", result);
        }
        public static string GetHtmlForSectionsIndex(List<Section> sections)
        {
            string result = "";

            foreach (Section section in sections)
            {
                result += string.Format("<li><a href=\"./{0}/{1}\">{2}</a></li>", section.Id, Tools.AppConfiguration.IndexFileName, section.Name);
            }

            return string.Format("<ul>{0}</ul>", result);
        }
        public static string GetHtmlForCategoriesIndex(List<Category> categories)
        {
            string result = "";

            foreach (Category category in categories)
            {
                result += string.Format("<li><a href=\"./{0}/{1}\">{2}</a></li>", category.Id, Tools.AppConfiguration.IndexFileName, category.Name);
            }

            return string.Format("<ul>{0}</ul>", result);
        }
    }
}
