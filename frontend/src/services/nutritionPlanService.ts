import { api } from './api';
import type { NutritionPlan } from '../types/nutritionPlan';

export const nutritionPlanService = {
  async getActivePlan(): Promise<NutritionPlan> {
    const { data } = await api.get('/nutritionplans/me');
    return data;
  },

  async getPlanHistory(): Promise<NutritionPlan[]> {
    const { data } = await api.get('/nutritionplans/me/history');
    return data ?? [];
  },

  async generateNewPlan(): Promise<NutritionPlan> {
    await api.post('/nutritionplans/me/generate', {});
    return nutritionPlanService.getActivePlan();
  },

  async downloadPlanPdf(): Promise<Blob> {
    const response = await api.get('/nutritionplans/me/active/pdf', {
      responseType: 'blob',
    });
    return new Blob([response.data], { type: 'application/pdf' });
  },
};
