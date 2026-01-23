using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace SanitizationAPI
{
    public static class PeopleNormalizer
    {
        public static People Normalize(People people)
        {
            return new People
            {
                Name = NormalizeName(people.Name),
                Email = NormalizeEmail(people.Email),
                Phone = NormalizePhone(people.Phone),
                Description = NormalizeDescription(people.Description),
                Content = NormalizeContent(people.Content)
            };
        }

        public static string NormalizeName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }
            name = name.Trim();
            name = Regex.Replace(name, @"\s+", " ");

            return name;
        }

        public static string NormalizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return string.Empty;
            }
            email = email.Trim().ToLowerInvariant();
            return email;
        }

        public static string NormalizePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return string.Empty;
            }
            phone = Regex.Replace(phone, @"\D", "");
            return phone;
        }

        public static string NormalizeDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return string.Empty;
            }
            description = Regex.Replace(description, @"[^a-zA-ZáéíóúÁÉÍÓÚÑñ\s]", "");
            description = Regex.Replace(description, @"\s+", " ");
            description = description.Trim();
            return description;
        }

        public static string NormalizeContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var scripts = doc.DocumentNode.SelectNodes("//script");
            if (scripts != null)
            {
                foreach (var script in scripts)
                {
                    script.Remove();
                }
            }

            return doc.DocumentNode.InnerHtml.Trim();
        }
    }
}
