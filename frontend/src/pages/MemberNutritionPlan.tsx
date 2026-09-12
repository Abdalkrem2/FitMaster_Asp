import React, { useState, useEffect } from 'react';
import { 
  Utensils, 
  Flame, 
  Droplet, 
  Wheat, 
  Pizza, 
  Download, 
  RefreshCw, 
  Clock, 
  ChevronDown, 
  ChevronUp, 
  AlertCircle
} from 'lucide-react';
import { nutritionPlanService } from '../services/nutritionPlanService';
import type { NutritionPlan, NutritionMeal } from '../types/nutritionPlan';

export default function MemberNutritionPlan() {
  const [plan, setPlan] = useState<NutritionPlan | null>(null);
  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [expandedMeals, setExpandedMeals] = useState<Record<number, boolean>>({});

  const fetchPlan = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await nutritionPlanService.getActivePlan();
      setPlan(data || null);
    } catch (err) {
      setPlan(null);
      setError(null);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPlan();
  }, []);

  const handleGeneratePlan = async () => {
    try {
      setGenerating(true);
      setError(null);
      const newPlan = await nutritionPlanService.generateNewPlan();
      setPlan(newPlan);
      setExpandedMeals({}); // Reset expanded state for new plan
    } catch (err) {
      setError('Failed to generate new nutrition plan. Please try again.');
    } finally {
      setGenerating(false);
    }
  };

  const handleDownloadPdf = async () => {
    try {
      const blob = await nutritionPlanService.downloadPlanPdf();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'nutrition-plan.pdf';
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } catch (err) {
      setError('Failed to download PDF. Please try again.');
    }
  };

  const toggleMeal = (mealId: number) => {
    setExpandedMeals(prev => ({
      ...prev,
      [mealId]: !prev[mealId]
    }));
  };

  if (loading) {
    return (
      <div className="min-h-[60vh] flex flex-col items-center justify-center space-y-4">
        <div className="w-12 h-12 border-4 border-green-100 border-t-green-600 rounded-full animate-spin"></div>
        <p className="text-slate-500 font-medium">Loading your nutrition plan...</p>
      </div>
    );
  }

  if (generating) {
    return (
      <div className="min-h-[60vh] flex flex-col items-center justify-center space-y-6">
        <div className="relative">
          <div className="w-16 h-16 border-4 border-green-100 border-t-green-600 rounded-full animate-spin"></div>
          <Utensils className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-6 h-6 text-green-600 animate-pulse" />
        </div>
        <div className="text-center space-y-2">
          <h3 className="text-lg font-semibold text-slate-800">Generating your plan...</h3>
          <p className="text-sm text-slate-500 max-w-sm mx-auto">
            Our AI 🤖 is crafting a personalized nutrition plan tailored to your body and fitness goals This may take up to 30 seconds
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-500">
      {/* Header Section */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
        <div>
          <h1 className="text-2xl font-bold text-slate-800 flex items-center gap-2">
            <Utensils className="w-6 h-6 text-green-600" />
            My Nutrition Plan
          </h1>
          {plan && (
            <p className="text-sm text-slate-500 mt-1">
              Goal: <span className="font-medium text-slate-700">{plan.goal.replace('_', ' ')}</span>
            </p>
          )}
        </div>

        <div className="flex flex-wrap items-center gap-3 w-full sm:w-auto">
          {plan && (
            <button
              onClick={handleDownloadPdf}
              className="flex-1 sm:flex-none flex items-center justify-center gap-2 px-4 py-2 bg-white border border-slate-200 text-slate-700 rounded-xl hover:bg-slate-50 hover:border-slate-300 transition-all font-medium text-sm"
            >
              <Download className="w-4 h-4" />
              Download PDF
            </button>
          )}
          <button
            onClick={handleGeneratePlan}
            className="flex-1 sm:flex-none flex items-center justify-center gap-2 px-4 py-2 bg-green-600 text-white rounded-xl hover:bg-green-700 transition-all font-medium text-sm shadow-sm shadow-green-600/20"
          >
            <RefreshCw className="w-4 h-4" />
            {plan ? 'Regenerate Plan' : 'Generate New Plan'}
          </button>
        </div>
      </div>

      {error && plan && (
        <div className="p-4 bg-rose-50 border border-rose-100 rounded-xl flex items-start gap-3 text-rose-600">
          <AlertCircle className="w-5 h-5 shrink-0 mt-0.5" />
          <p className="text-sm font-medium">{error}</p>
        </div>
      )}

      {!plan ? (
        <div className="bg-white rounded-2xl shadow-sm border border-slate-100 p-12 text-center flex flex-col items-center justify-center space-y-4">
          {error && (
            <div className="mb-4 p-4 bg-rose-50 border border-rose-100 rounded-xl flex items-start gap-3 text-rose-600 text-left w-full max-w-md mx-auto">
              <AlertCircle className="w-5 h-5 shrink-0 mt-0.5" />
              <p className="text-sm font-medium">{error}</p>
            </div>
          )}
          <div className="w-20 h-20 bg-green-50 rounded-full flex items-center justify-center mb-2">
            <Utensils className="w-10 h-10 text-green-600" />
          </div>
          <h2 className="text-xl font-bold text-slate-800">No nutrition plan yet</h2>
          <p className="text-slate-500 max-w-md mx-auto text-sm">
            You don't have an active nutrition plan. Generate one now to get personalized meal recommendations based on your fitness profile.
          </p>
          <button
            onClick={handleGeneratePlan}
            className="mt-4 px-6 py-3 bg-green-600 text-white rounded-xl hover:bg-green-700 transition-all font-medium flex items-center gap-2 shadow-sm shadow-green-600/20"
          >
            <RefreshCw className="w-5 h-5" />
            Generate Plan Now
          </button>
        </div>
      ) : (
        <>
          {/* Daily Summary Cards */}
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div className="bg-white p-5 rounded-2xl shadow-sm border border-green-100 flex items-center gap-4">
              <div className="w-12 h-12 bg-green-50 rounded-xl flex items-center justify-center shrink-0">
                <Flame className="w-6 h-6 text-green-600" />
              </div>
              <div>
                <p className="text-sm font-medium text-slate-500">Calories</p>
                <p className="text-2xl font-bold text-slate-800">{plan.dailyCalories} <span className="text-sm font-medium text-slate-500">kcal</span></p>
              </div>
            </div>

            <div className="bg-white p-5 rounded-2xl shadow-sm border border-blue-100 flex items-center gap-4">
              <div className="w-12 h-12 bg-blue-50 rounded-xl flex items-center justify-center shrink-0">
                <Pizza className="w-6 h-6 text-blue-600" />
              </div>
              <div>
                <p className="text-sm font-medium text-slate-500">Protein</p>
                <p className="text-2xl font-bold text-slate-800">{plan.proteinGrams} <span className="text-sm font-medium text-slate-500">g</span></p>
              </div>
            </div>

            <div className="bg-white p-5 rounded-2xl shadow-sm border border-orange-100 flex items-center gap-4">
              <div className="w-12 h-12 bg-orange-50 rounded-xl flex items-center justify-center shrink-0">
                <Wheat className="w-6 h-6 text-orange-600" />
              </div>
              <div>
                <p className="text-sm font-medium text-slate-500">Carbs</p>
                <p className="text-2xl font-bold text-slate-800">{plan.carbsGrams} <span className="text-sm font-medium text-slate-500">g</span></p>
              </div>
            </div>

            <div className="bg-white p-5 rounded-2xl shadow-sm border border-yellow-100 flex items-center gap-4">
              <div className="w-12 h-12 bg-yellow-50 rounded-xl flex items-center justify-center shrink-0">
                <Droplet className="w-6 h-6 text-yellow-500" />
              </div>
              <div>
                <p className="text-sm font-medium text-slate-500">Fat</p>
                <p className="text-2xl font-bold text-slate-800">{plan.fatGrams} <span className="text-sm font-medium text-slate-500">g</span></p>
              </div>
            </div>
          </div>

          {/* Meals Section */}
          <div className="space-y-4">
            <h2 className="text-lg font-bold text-slate-800 px-1">Daily Meals</h2>
            
            {plan.meals.map((meal: NutritionMeal) => {
              const isExpanded = expandedMeals[meal.id];
              
              return (
                <div key={meal.id} className="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden transition-all duration-200 hover:border-green-200">
                  <div 
                    className="p-5 flex items-center justify-between cursor-pointer select-none"
                    onClick={() => toggleMeal(meal.id)}
                  >
                    <div className="flex items-center gap-4">
                      <div className="w-10 h-10 bg-slate-50 rounded-full flex items-center justify-center shrink-0">
                        <Utensils className="w-5 h-5 text-slate-400" />
                      </div>
                      <div>
                        <h3 className="text-base font-bold text-slate-800 flex items-center gap-2">
                          {meal.name}
                          <span className="text-xs font-medium px-2 py-0.5 bg-slate-100 text-slate-600 rounded-full">
                            {meal.mealTime}
                          </span>
                        </h3>
                        <p className="text-sm text-slate-500 flex items-center gap-1 mt-1">
                          <Flame className="w-3.5 h-3.5 text-orange-400" />
                          {meal.totalCalories} kcal
                          <span className="mx-2 text-slate-300">•</span>
                          <Clock className="w-3.5 h-3.5 text-blue-400" />
                          {meal.prepTime} prep
                        </p>
                      </div>
                    </div>
                    <div className="p-2 bg-slate-50 rounded-full text-slate-400">
                      {isExpanded ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
                    </div>
                  </div>

                  {isExpanded && (
                    <div className="px-5 pb-5 pt-0 border-t border-slate-100 mt-2">
                      <div className="mt-4 space-y-6">
                        {/* Foods Table */}
                        <div>
                          <h4 className="text-sm font-bold text-slate-700 mb-3 uppercase tracking-wider">Ingredients</h4>
                          <div className="overflow-x-auto">
                            <table className="w-full text-sm text-left">
                              <thead className="text-xs text-slate-500 bg-slate-50 rounded-lg">
                                <tr>
                                  <th className="px-4 py-2 font-medium rounded-l-lg">Food</th>
                                  <th className="px-4 py-2 font-medium">Amount</th>
                                  <th className="px-4 py-2 font-medium">Calories</th>
                                  <th className="px-4 py-2 font-medium">Protein</th>
                                  <th className="px-4 py-2 font-medium">Carbs</th>
                                  <th className="px-4 py-2 font-medium rounded-r-lg">Fat</th>
                                </tr>
                              </thead>
                              <tbody className="divide-y divide-slate-50">
                                {meal.foods.map(food => (
                                  <tr key={food.id} className="hover:bg-slate-50/50">
                                    <td className="px-4 py-3 font-medium text-slate-700">{food.name}</td>
                                    <td className="px-4 py-3 text-slate-600">{food.amount}</td>
                                    <td className="px-4 py-3 text-slate-600">{food.calories} kcal</td>
                                    <td className="px-4 py-3 text-blue-600">{food.proteinGrams}g</td>
                                    <td className="px-4 py-3 text-orange-600">{food.carbsGrams}g</td>
                                    <td className="px-4 py-3 text-yellow-600">{food.fatGrams}g</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          </div>
                        </div>

                        {/* Preparation Steps */}
                        {meal.recipeSteps && meal.recipeSteps.length > 0 && (
                          <div>
                            <h4 className="text-sm font-bold text-slate-700 mb-3 uppercase tracking-wider">Preparation</h4>
                            <div className="space-y-3">
                              {meal.recipeSteps.map(step => (
                                <div key={step.id} className="flex items-start gap-3">
                                  <div className="w-6 h-6 rounded-full bg-green-100 text-green-700 flex items-center justify-center shrink-0 text-xs font-bold mt-0.5">
                                    {step.stepOrder}
                                  </div>
                                  <p className="text-sm text-slate-600 leading-relaxed">
                                    {step.instruction}
                                  </p>
                                </div>
                              ))}
                            </div>
                          </div>
                        )}
                      </div>
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </>
      )}
    </div>
  );
}
