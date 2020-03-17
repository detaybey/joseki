import { InfrastructureComponentSummary } from '@/models';

export class InfrastructureOverview {
    /// Overall infrastructure summary.
    overall: InfrastructureComponentSummary = new InfrastructureComponentSummary();
    /// Separate summary for each involved component.
    components: InfrastructureComponentSummary[] = [];

    public static GenerateFromDiff(overall: InfrastructureComponentSummary, components: InfrastructureComponentSummary[])
      : InfrastructureOverview
    {
      let data = { overall: overall, components: components }      
      return InfrastructureOverview.GenerateFromData(data);
    }

    public static GenerateFromData(data: any): InfrastructureOverview {
      let result = new InfrastructureOverview();
      result.overall = data.overall;
      // reverse and slice overall history
      if(result.overall.scoreHistory) {
        result.overall.scoreHistory = result.overall.scoreHistory.reverse().slice(0, 14);
      }
      result.components = data.components;

      // generate sections for components
      for (let i = 0; i < result.components.length; i++) {

        if (result.components[i].component.category === 'Subscription') {
          result.components[i].component.category = 'Azure Subscription';
        }
        result.components[i].sections = InfrastructureComponentSummary.getSections(result.components[i].current);
        result.components[i].scoreHistory = result.components[i].scoreHistory.reverse().slice(0, 14);

        // truncate scan times from component history
        for (let j = 0; j < result.components[i].scoreHistory.length; j++) {
          let inputDate = result.components[i].scoreHistory[j].recordedAt.toString();
          let dateReplacement = new Date(inputDate.split('T')[0] + 'T00:00:00');
          result.components[i].scoreHistory[j].recordedAt = dateReplacement;
        }
      }

      return result;
    }
}