import React, { useState, useEffect } from 'react';
import { 
  History, 
  Dumbbell, 
  Utensils, 
  AlertCircle,
  Calendar,
  ChevronDown,
  ChevronUp,
  Target,
  BarChart3,
  Zap,
  Flame,
  Pizza,
  Wheat,
  Droplet
} from 'lucide-react';
import { workoutPlanService } from '../services/workoutPlanService';
import { nutritionPlanService } from '../services/nutritionPlanService';
import type { WorkoutPlan, WorkoutDay } from '../types/workoutPlan';
import type { NutritionPlan, NutritionMeal } from '../types/nutritionPlan';

export default function MemberPlanHistory() {
  const [activeTab, setActiveTab] = useState<'workout' | 'nutrition'>('workout');
  
  const [workoutPlans, setWorkoutPlans] = useState<WorkoutPlan[]>([]);
  const [nutritionPlans, setNutritionPlans] = useState<NutritionPlan[]>([]);
  
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  const [expandedPlanId, setExpandedPlanId] = useState<number | null>(null);

  useEffect(() => {
    fetchHistory();
  }, []);

  const fetchHistory = async () => {
    try {
      setLoading(true);
      setError(null);
      
      // Fetch both histories concurrently
      const [workouts, nutritions] = await Promise.all([
        workoutPlanService.getPlanHistory().catch(() => []),
        nutritionPlanService.getPlanHistory().catch(() => [])
      ]);

      // Sort by date (newest first), ensuring ACTIVE is always at the top
      const sortPlans = (plans: any[]) => {
        return plans.sort((a, b) => {
          if (a.status === 'ACTIVE' && b.status !== 'ACTIVE') return -1;
          if (a.status !== 'ACTIVE' && b.status === 'ACTIVE') return 1;
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
        });
      };

      setWorkoutPlans(sortPlans(workouts));
      setNutritionPlans(sortPlans(nutritions));
      
    } catch (err) {
      setError('Failed to load plan history. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (isoString: string) => {
    return new Date(isoString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  const toggleExpand = (planId: number) => {
    setExpandedPlanId(prev => prev === planId ? null : planId);
  };

  const renderWorkoutPlanDetails = (plan: WorkoutPlan) => {
    return (
      <div className="mt-4 space-y-4 border-t border-slate-100 pt-4">
        {plan.days.map((day: WorkoutDay) => (
          <div key={day.dayNumber} className="bg-slate-50 rounded-xl p-4">
            <h4 className="font-bold text-slate-700 text-sm mb-3">
              Day {day.dayNumber}: {day.muscleGroupLabel}
            </h4>
            <div className="space-y-2">
              {day.exercises.map(exercise => (
                <div key={exercise.id} className="flex justify-between items-center text-sm bg-white p-2.5 rounded-lg border border-slate-100">
                  <span className="font-medium text-slate-700">{exercise.exerciseName}</span>
                  <div className="text-slate-500 text-xs flex gap-3">
                    <span>{exercise.sets} sets</span>
                    <span>{exercise.reps} reps</span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    );
  };

  const renderNutritionPlanDetails = (plan: NutritionPlan) => {
    return (
      <div className="mt-4 space-y-4 border-t border-slate-100 pt-4">
        {plan.meals.map((meal: NutritionMeal) => (
          <div key={meal.id} className="bg-slate-50 rounded-xl p-4">
            <div className="flex justify-between items-center mb-3">
              <h4 className="font-bold text-slate-700 text-sm flex items-center gap-2">
                {meal.name}
                <span className="text-xs font-normal text-slate-500 bg-white px-2 py-0.5 rounded-full border border-slate-200">
                  {meal.mealTime}
                </span>
              </h4>
              <span className="text-xs font-medium text-orange-600 bg-orange-50 px-2 py-0.5 rounded-full">
                {meal.totalCalories} kcal
              </span>
            </div>
            <div className="space-y-2">
              {meal.foods.map(food => (
                <div key={food.id} className="flex justify-between items-center text-sm bg-white p-2.5 rounded-lg border border-slate-100">
                  <span className="font-medium text-slate-700">{food.name} <span className="text-slate-400 font-normal">({food.amount})</span></span>
                  <div className="text-slate-500 text-xs flex gap-2">
                    <span className="text-blue-600">{food.proteinGrams}p</span>
                    <span className="text-orange-600">{food.carbsGrams}c</span>
                    <span className="text-yellow-600">{food.fatGrams}f</span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    );
  };

  return (
    <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-500">
      {/* Header Section */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
        <div>
          <h1 className="text-2xl font-bold text-slate-800 flex items-center gap-2">
            <History className="w-6 h-6 text-slate-600" />
            Plan History
          </h1>
          <p className="text-sm text-slate-500 mt-1">
            View your past and present training and nutrition programmes
          </p>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex bg-white rounded-xl shadow-sm border border-slate-100 p-1.5 w-fit">
        <button
          onClick={() => { setActiveTab('workout'); setExpandedPlanId(null); }}
          className={`flex items-center gap-2 px-5 py-2.5 rounded-lg text-sm font-semibold transition-all ${
            activeTab === 'workout' 
              ? 'bg-indigo-50 text-indigo-600' 
              : 'text-slate-500 hover:text-slate-700 hover:bg-slate-50'
          }`}
        >
          <Dumbbell className="w-4 h-4" />
          Workout Plans
        </button>
        <button
          onClick={() => { setActiveTab('nutrition'); setExpandedPlanId(null); }}
          className={`flex items-center gap-2 px-5 py-2.5 rounded-lg text-sm font-semibold transition-all ${
            activeTab === 'nutrition' 
              ? 'bg-green-50 text-green-600' 
              : 'text-slate-500 hover:text-slate-700 hover:bg-slate-50'
          }`}
        >
          <Utensils className="w-4 h-4" />
          Nutrition Plans
        </button>
      </div>

      {/* Loading & Error States */}
      {loading && (
        <div className="min-h-[40vh] flex flex-col items-center justify-center space-y-4">
          <div className="w-10 h-10 border-4 border-slate-100 border-t-slate-600 rounded-full animate-spin"></div>
          <p className="text-slate-500 font-medium">Loading history...</p>
        </div>
      )}

      {error && !loading && (
        <div className="p-4 bg-rose-50 border border-rose-100 rounded-xl flex items-start gap-3 text-rose-600">
          <AlertCircle className="w-5 h-5 shrink-0 mt-0.5" />
          <p className="text-sm font-medium">{error}</p>
        </div>
      )}

      {/* Content */}
      {!loading && !error && (
        <div className="space-y-4">
          
          {/* Workout Plans */}
          {activeTab === 'workout' && (
            <>
              {workoutPlans.length === 0 ? (
                <div className="bg-white rounded-2xl border border-slate-100 p-12 text-center">
                  <Dumbbell className="w-12 h-12 text-slate-200 mx-auto mb-4" />
                  <h3 className="text-lg font-bold text-slate-800">No workout plans yet</h3>
                  <p className="text-slate-500 mt-2 text-sm">Generate your first workout plan to see it here.</p>
                </div>
              ) : (
                workoutPlans.map(plan => {
                  const isActive = plan.status === 'ACTIVE';
                  const isExpanded = expandedPlanId === plan.id;
                  
                  return (
                    <div 
                      key={plan.id} 
                      className={`bg-white rounded-2xl shadow-sm border transition-all overflow-hidden ${
                        isActive ? 'border-indigo-400 ring-1 ring-indigo-400/20' : 'border-slate-200 hover:border-slate-300'
                      }`}
                    >
                      <div 
                        className="p-5 cursor-pointer select-none"
                        onClick={() => toggleExpand(plan.id)}
                      >
                        <div className="flex flex-col sm:flex-row justify-between sm:items-center gap-4">
                          <div className="space-y-2">
                            <div className="flex items-center gap-3">
                              <h3 className="text-lg font-bold text-slate-800">{plan.name}</h3>
                              {isActive ? (
                                <span className="px-2.5 py-0.5 rounded-full bg-emerald-50 text-emerald-600 text-xs font-bold uppercase tracking-wider">Active</span>
                              ) : (
                                <span className="px-2.5 py-0.5 rounded-full bg-slate-100 text-slate-500 text-xs font-bold uppercase tracking-wider">Archived</span>
                              )}
                            </div>
                            <div className="flex flex-wrap gap-2">
                              <span className="flex items-center gap-1.5 text-xs font-medium px-2.5 py-1 rounded-md bg-slate-50 text-slate-600 border border-slate-100">
                                <Calendar className="w-3.5 h-3.5 text-slate-400" />
                                {formatDate(plan.createdAt)}
                              </span>
                              <span className="flex items-center gap-1.5 text-xs font-medium px-2.5 py-1 rounded-md bg-indigo-50 text-indigo-600 border border-indigo-100/50">
                                <BarChart3 className="w-3.5 h-3.5" />
                                {plan.splitType.replace(/_/g, ' ')}
                              </span>
                              <span className="flex items-center gap-1.5 text-xs font-medium px-2.5 py-1 rounded-md bg-violet-50 text-violet-600 border border-violet-100/50">
                                <Target className="w-3.5 h-3.5" />
                                {plan.goal.replace(/_/g, ' ')}
                              </span>
                              <span className="flex items-center gap-1.5 text-xs font-medium px-2.5 py-1 rounded-md bg-slate-50 text-slate-600 border border-slate-100">
                                <Zap className="w-3.5 h-3.5 text-amber-500" />
                                {plan.level.replace(/_/g, ' ')}
                              </span>
                            </div>
                          </div>
                          
                          <div className="flex items-center justify-between sm:justify-end gap-4 sm:min-w-[120px]">
                            <div className="text-sm font-medium text-slate-500">
                              {plan.days.length} Days
                            </div>
                            <div className="p-2 bg-slate-50 rounded-full text-slate-400">
                              {isExpanded ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
                            </div>
                          </div>
                        </div>
                      </div>
                      
                      {isExpanded && (
                        <div className="px-5 pb-5">
                          {renderWorkoutPlanDetails(plan)}
                        </div>
                      )}
                    </div>
                  );
                })
              )}
            </>
          )}

          {/* Nutrition Plans */}
          {activeTab === 'nutrition' && (
            <>
              {nutritionPlans.length === 0 ? (
                <div className="bg-white rounded-2xl border border-slate-100 p-12 text-center">
                  <Utensils className="w-12 h-12 text-slate-200 mx-auto mb-4" />
                  <h3 className="text-lg font-bold text-slate-800">No nutrition plans yet</h3>
                  <p className="text-slate-500 mt-2 text-sm">Generate your first nutrition plan to see it here.</p>
                </div>
              ) : (
                nutritionPlans.map(plan => {
                  const isActive = plan.status === 'ACTIVE';
                  const isExpanded = expandedPlanId === plan.id;
                  
                  return (
                    <div 
                      key={plan.id} 
                      className={`bg-white rounded-2xl shadow-sm border transition-all overflow-hidden ${
                        isActive ? 'border-green-400 ring-1 ring-green-400/20' : 'border-slate-200 hover:border-slate-300'
                      }`}
                    >
                      <div 
                        className="p-5 cursor-pointer select-none"
                        onClick={() => toggleExpand(plan.id)}
                      >
                        <div className="flex flex-col sm:flex-row justify-between sm:items-center gap-4">
                          <div className="space-y-3">
                            <div className="flex items-center gap-3">
                              <h3 className="text-lg font-bold text-slate-800 flex items-center gap-2">
                                Nutrition Plan
                              </h3>
                              {isActive ? (
                                <span className="px-2.5 py-0.5 rounded-full bg-emerald-50 text-emerald-600 text-xs font-bold uppercase tracking-wider">Active</span>
                              ) : (
                                <span className="px-2.5 py-0.5 rounded-full bg-slate-100 text-slate-500 text-xs font-bold uppercase tracking-wider">Archived</span>
                              )}
                            </div>
                            
                            <div className="flex flex-wrap gap-2">
                              <span className="flex items-center gap-1.5 text-xs font-medium px-2.5 py-1 rounded-md bg-slate-50 text-slate-600 border border-slate-100">
                                <Calendar className="w-3.5 h-3.5 text-slate-400" />
                                {formatDate(plan.createdAt)}
                              </span>
                              <span className="flex items-center gap-1.5 text-xs font-medium px-2.5 py-1 rounded-md bg-green-50 text-green-700 border border-green-100/50">
                                <Target className="w-3.5 h-3.5" />
                                {plan.goal.replace(/_/g, ' ')}
                              </span>
                            </div>

                            {/* Macro Summary Mini Cards */}
                            <div className="flex flex-wrap gap-3 pt-1">
                              <div className="flex items-center gap-1.5 text-sm font-semibold text-slate-700">
                                <Flame className="w-4 h-4 text-green-500" />
                                {plan.dailyCalories} <span className="text-xs font-normal text-slate-500">kcal</span>
                              </div>
                              <div className="w-px h-4 bg-slate-200 my-auto hidden sm:block"></div>
                              <div className="flex items-center gap-1.5 text-sm font-semibold text-slate-700">
                                <Pizza className="w-4 h-4 text-blue-500" />
                                {plan.proteinGrams}g <span className="text-xs font-normal text-slate-500">pro</span>
                              </div>
                              <div className="flex items-center gap-1.5 text-sm font-semibold text-slate-700">
                                <Wheat className="w-4 h-4 text-orange-500" />
                                {plan.carbsGrams}g <span className="text-xs font-normal text-slate-500">carb</span>
                              </div>
                              <div className="flex items-center gap-1.5 text-sm font-semibold text-slate-700">
                                <Droplet className="w-4 h-4 text-yellow-500" />
                                {plan.fatGrams}g <span className="text-xs font-normal text-slate-500">fat</span>
                              </div>
                            </div>
                          </div>
                          
                          <div className="flex items-center justify-between sm:justify-end gap-4 sm:min-w-[120px]">
                            <div className="text-sm font-medium text-slate-500">
                              {plan.meals.length} Meals
                            </div>
                            <div className="p-2 bg-slate-50 rounded-full text-slate-400">
                              {isExpanded ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
                            </div>
                          </div>
                        </div>
                      </div>
                      
                      {isExpanded && (
                        <div className="px-5 pb-5">
                          {renderNutritionPlanDetails(plan)}
                        </div>
                      )}
                    </div>
                  );
                })
              )}
            </>
          )}

        </div>
      )}
    </div>
  );
}
