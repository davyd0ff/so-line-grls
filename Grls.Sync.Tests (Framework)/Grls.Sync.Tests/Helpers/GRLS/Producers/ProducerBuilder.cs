using Core.Models;
using Core.Models.Medicaments;

namespace Grls.Sync.Tests.Helpers.GRLS.Producers
{
    public class ProducerBuilder
    {
        private Producer _producer;

        public ProducerBuilder()
        {
            this._producer = new Producer();

            if (this._producer.Organization == null)
            {
                this._producer.Organization = new OldOrganization();
                this._producer.Organization.Address = "";
            }

            if (this._producer.Organization.Name == null)
            {
                this._producer.Organization.Name = new Name();
                this._producer.Organization.Name.Russian = "";
                this._producer.Organization.Name.English = "";
            }

            if (this._producer.Country == null)
            {
                this._producer.Country = new OldCountry();
                this._producer.Country.RussianName = "";
            }
        }

        public ProducerBuilder WithStage(string stage)
        {
            this._producer.Stage = new Stage() { Code = stage };


            return this;
        }

        public ProducerBuilder WithRuOrgName(string name)
        {
            this._producer.Organization.Name.Russian = name;

            return this;
        }

        public ProducerBuilder WithEnOrgName(string name)
        {
            this._producer.Organization.Name.English = name;

            return this;
        }

        public ProducerBuilder WithRuCountryName(string name)
        {
            if (this._producer.Country == null)
            {
                this._producer.Country = new OldCountry();
            }

            this._producer.Country.RussianName = name;


            return this;
        }

        public ProducerBuilder WithEnCountryName(string name)
        {
            if (this._producer.Country == null)
            {
                this._producer.Country = new OldCountry();
            }

            this._producer.Country.EnglishName = name;


            return this;
        }

        public ProducerBuilder WithOrgAddress(string address)
        {
            if (this._producer.Organization == null)
            {
                this._producer.Organization = new OldOrganization();
            }

            this._producer.Organization.Address = address;


            return this;
        }

        public Producer Please()
        {
            return this._producer;
        }

        public static implicit operator Producer(ProducerBuilder builder)
        {
            return builder._producer;
        }
    }
}
