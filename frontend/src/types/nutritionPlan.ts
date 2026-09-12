export type NutritionGoal = 'MUSCLE_GAIN' | 'WEIGHT_LOSS' | 'ENDURANCE' | 'GENERAL_FITNESS';
export type PlanStatus = 'ACTIVE' | 'ARCHIVED';

export interface NutritionFood {
  id: number;
  name: string;
  amount: string;
  calories: number;
  proteinGrams: number;
  carbsGrams: number;
  fatGrams: number;
}

export interface NutritionRecipeStep {
  id: number;
  stepOrder: number;
  instruction: string;
}

export interface NutritionMeal {
  id: number;
  name: string;
  mealTime: string;
  prepTime: string;
  totalCalories: number;
  foods: NutritionFood[];
  recipeSteps: NutritionRecipeStep[];
}

export interface NutritionPlan {
  id: number;
  goal: NutritionGoal | string;
  dailyCalories: number;
  proteinGrams: number;
  carbsGrams: number;
  fatGrams: number;
  status: PlanStatus | string;
  createdAt: string;
  meals: NutritionMeal[];
}
