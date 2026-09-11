import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Activity,
  Target,
  TrendingUp,
  Zap,
  Scale,
  Ruler,
  Calendar,
  Shield,
  CheckCircle2,
  ArrowRight,
  ArrowLeft,
  Dumbbell,
  Sparkles,
  LayoutGrid,
  Heart,
  Droplets,
  ActivitySquare,
} from "lucide-react";
import { memberService } from "../services/memberService";
import type {
  FitnessGoal,
  FitnessLevel,
  TrainingStyle,
  SplitType,
  InjuryType,
  AllergyType,
} from "../types/member";

interface OnboardingData {
  goal?: FitnessGoal;
  fitnessLevel?: FitnessLevel;
  trainingStyle?: TrainingStyle;
  splitType?: SplitType;
  daysPerWeek: number;
  weight?: number;
  height?: number;
  age?: number;
  injuries: InjuryType[];
  hasDiabetes: boolean;
  hasHeartConditions: boolean;
  hasHypertension: boolean;
  allergies: AllergyType[];
}

// Only three splits are supported by the workout generator - the old
// BRO_SPLIT_4DAY/5DAY options no longer exist on the backend (Phase 6
// redesigned the generator around FullBody/UpperLower/PushPullLegs only).
const splitDays: Record<SplitType, number> = {
  FULL_BODY: 3,
  UPPER_LOWER: 4,
  PUSH_PULL_LEGS: 6,
};

const splitTypes: {
  value: SplitType;
  label: string;
  desc: string;
  days: number;
}[] = [
  {
    value: "FULL_BODY",
    label: "Full Body",
    desc: "Train all muscles each session",
    days: 3,
  },
  {
    value: "UPPER_LOWER",
    label: "Upper / Lower",
    desc: "Alternate upper & lower body",
    days: 4,
  },
  {
    value: "PUSH_PULL_LEGS",
    label: "Push Pull Legs",
    desc: "Push / Pull / Legs × 2",
    days: 6,
  },
];

const TOTAL_STEPS = 7;

const steps = [
  { label: "Your Goal", desc: "What do you want to achieve?" },
  { label: "Fitness Level", desc: "How experienced are you?" },
  { label: "Training Style", desc: "How do you like to train?" },
  { label: "Split Type", desc: "Choose your training split" },
  { label: "Body Info", desc: "Optional measurements" },
  { label: "Injuries", desc: "Areas to work around" },
  { label: "Health & Diet", desc: "Conditions & allergies" },
];

const goals: {
  value: FitnessGoal;
  label: string;
  desc: string;
  icon: typeof Target;
  gradient: string;
}[] = [
  {
    value: "MUSCLE_GAIN",
    label: "Muscle Gain",
    desc: "Build strength & size",
    icon: TrendingUp,
    gradient: "from-indigo-500 to-violet-600",
  },
  {
    value: "WEIGHT_LOSS",
    label: "Weight Loss",
    desc: "Burn fat & get lean",
    icon: Scale,
    gradient: "from-rose-400 to-pink-600",
  },
  {
    value: "ENDURANCE",
    label: "Endurance",
    desc: "Boost stamina & cardio",
    icon: Zap,
    gradient: "from-amber-400 to-orange-500",
  },
  {
    value: "GENERAL_FITNESS",
    label: "General Fitness",
    desc: "Stay active & healthy",
    icon: Target,
    gradient: "from-emerald-400 to-teal-500",
  },
];

const levels: {
  value: FitnessLevel;
  label: string;
  desc: string;
  years: string;
}[] = [
  {
    value: "BEGINNER",
    label: "Beginner",
    desc: "Just getting started",
    years: "0–1 year",
  },
  {
    value: "INTERMEDIATE",
    label: "Intermediate",
    desc: "Know the basics well",
    years: "1–3 years",
  },
  {
    value: "ADVANCED",
    label: "Advanced",
    desc: "Training consistently",
    years: "3+ years",
  },
];

const styles: {
  value: TrainingStyle;
  label: string;
  desc: string;
  icon: typeof Dumbbell;
}[] = [
  {
    value: "STRENGTH",
    label: "Strength",
    desc: "Heavy compound lifts",
    icon: Dumbbell,
  },
  {
    value: "HYPERTROPHY",
    label: "Hypertrophy",
    desc: "Muscle building focus",
    icon: TrendingUp,
  },
  {
    value: "CIRCUIT",
    label: "Circuit",
    desc: "High intensity rounds",
    icon: Zap,
  },
];

const injuryOptions: InjuryType[] = [
  "KNEE",
  "SHOULDER",
  "LOWER_BACK",
  "UPPER_BACK",
  "WRIST",
  "ANKLE",
  "NECK",
  "ELBOW",
  "HIP",
];

export default function MemberOnboarding() {
  const navigate = useNavigate();
  const [step, setStep] = useState(0);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [data, setData] = useState<OnboardingData>({
    daysPerWeek: 3,
    injuries: [],
    hasDiabetes: false,
    hasHeartConditions: false,
    hasHypertension: false,
    allergies: [],
  });

  const set = <K extends keyof OnboardingData>(
    key: K,
    val: OnboardingData[K],
  ) => setData((d) => ({ ...d, [key]: val }));

  const toggleInjury = (injury: InjuryType) =>
    setData((d) => ({
      ...d,
      injuries: d.injuries.includes(injury)
        ? d.injuries.filter((i) => i !== injury)
        : [...d.injuries, injury],
    }));

  const toggleAllergy = (allergy: AllergyType) =>
    setData((d) => ({
      ...d,
      allergies: d.allergies.includes(allergy)
        ? d.allergies.filter((a) => a !== allergy)
        : [...d.allergies, allergy],
    }));

  const canAdvance = () => {
    if (step === 1) return !!data.goal;
    if (step === 2) return !!data.fitnessLevel;
    if (step === 3) return !!data.trainingStyle;
    if (step === 4) return !!data.splitType;
    return true;
  };

  const selectSplit = (value: SplitType) => {
    setData((d) => ({ ...d, splitType: value, daysPerWeek: splitDays[value] }));
  };

  const handleFinish = async () => {
    setSaving(true);
    setError(null);
    try {
      await memberService.updateMyProfile({
        goal: data.goal!,
        fitnessLevel: data.fitnessLevel!,
        trainingStyle: data.trainingStyle!,
        splitType: data.splitType!,
        injuries:
          data.injuries && data.injuries.length > 0 ? data.injuries : null,
        weight: data.weight ?? null,
        height: data.height ?? null,
        age: data.age ?? null,
        hasDiabetes: data.hasDiabetes,
        hasHeartConditions: data.hasHeartConditions,
        hasHypertension: data.hasHypertension,
        allergies: data.allergies.length > 0 ? data.allergies : undefined,
      });
      navigate("/member-dashboard");
    } catch {
      setError("Something went wrong. Please try again.");
      setSaving(false);
    }
  };

  const progress = step === 0 ? 0 : Math.round((step / TOTAL_STEPS) * 100);

  return (
    <div className="min-h-screen flex flex-col md:flex-row bg-slate-50">
      {/* ═══ LEFT PANEL (desktop only) ═══ */}
      <div className="hidden md:flex md:w-72 lg:w-80 xl:w-96 shrink-0 bg-gradient-to-br from-indigo-600 via-indigo-500 to-violet-600 flex-col justify-between p-10 relative overflow-hidden">
        <div className="absolute top-0 right-0 w-64 h-64 bg-white/5 rounded-full -translate-y-1/2 translate-x-1/2" />
        <div className="absolute bottom-0 left-0 w-48 h-48 bg-white/5 rounded-full translate-y-1/2 -translate-x-1/2" />

        <div className="relative z-10">
          <div className="flex items-center gap-3 mb-12">
            <div className="w-10 h-10 rounded-xl bg-white/20 backdrop-blur-sm border border-white/30 flex items-center justify-center">
              <Activity className="w-5 h-5 text-white" />
            </div>
            <span className="text-xl font-bold text-white">FitMaster</span>
          </div>

          {step === 0 ? (
            <div>
              <div className="w-16 h-16 rounded-2xl bg-white/15 border border-white/20 flex items-center justify-center mb-6">
                <Sparkles className="w-8 h-8 text-white" />
              </div>
              <h2 className="text-2xl font-bold text-white mb-3">
                Let's set you up
              </h2>
              <p className="text-indigo-200 text-sm leading-relaxed">
                Answer a few quick questions and we'll build a personalised
                fitness plan just for you.
              </p>
            </div>
          ) : (
            <div>
              <p className="text-indigo-200 text-xs font-semibold uppercase tracking-wider mb-6">
                Your progress
              </p>
              <div className="space-y-2">
                {steps.map((s, i) => {
                  const idx = i + 1;
                  const done = idx < step;
                  const active = idx === step;
                  return (
                    <div
                      key={s.label}
                      className={`flex items-center gap-3 px-3 py-2.5 rounded-xl transition-all ${active ? "bg-white/15 border border-white/20" : ""}`}
                    >
                      <div
                        className={`w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold shrink-0 ${
                          done
                            ? "bg-white text-indigo-600"
                            : active
                              ? "bg-white/30 text-white border-2 border-white/60"
                              : "bg-white/10 text-indigo-300"
                        }`}
                      >
                        {done ? <CheckCircle2 className="w-4 h-4" /> : idx}
                      </div>
                      <div>
                        <p
                          className={`text-sm font-semibold leading-none ${active ? "text-white" : done ? "text-indigo-200" : "text-indigo-300/60"}`}
                        >
                          {s.label}
                        </p>
                        {active && (
                          <p className="text-xs text-indigo-200 mt-0.5">
                            {s.desc}
                          </p>
                        )}
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          )}
        </div>

        {step > 0 && (
          <div className="relative z-10">
            <div className="flex justify-between text-xs text-indigo-200 mb-2">
              <span>Progress</span>
              <span>{progress}%</span>
            </div>
            <div className="h-1.5 bg-white/20 rounded-full overflow-hidden">
              <div
                className="h-full bg-white rounded-full transition-all duration-500"
                style={{ width: `${progress}%` }}
              />
            </div>
          </div>
        )}
      </div>

      {/* ═══ RIGHT / MAIN CONTENT ═══ */}
      <div className="flex-1 flex flex-col min-h-screen">
        {/* Mobile top bar */}
        <div className="md:hidden sticky top-0 z-20 bg-white/90 backdrop-blur-md border-b border-slate-100">
          <div className="px-4 h-14 flex items-center justify-between">
            <div className="flex items-center gap-2">
              <div className="w-7 h-7 rounded-lg bg-gradient-to-br from-indigo-500 to-violet-600 flex items-center justify-center">
                <Activity className="w-3.5 h-3.5 text-white" />
              </div>
              <span className="text-base font-bold bg-gradient-to-r from-indigo-600 to-violet-600 bg-clip-text text-transparent">
                FitMaster
              </span>
            </div>
            {step > 0 && (
              <div className="flex items-center gap-2 text-sm text-slate-400">
                <div className="h-1.5 w-24 bg-slate-100 rounded-full overflow-hidden">
                  <div
                    className="h-full bg-gradient-to-r from-indigo-500 to-violet-600 rounded-full transition-all duration-500"
                    style={{ width: `${progress}%` }}
                  />
                </div>
                <span className="text-xs font-medium">
                  {step}/{TOTAL_STEPS}
                </span>
              </div>
            )}
          </div>
        </div>

        {/* Content */}
        <div className="flex-1 flex items-center justify-center px-4 sm:px-8 md:px-8 lg:px-12 xl:px-20 py-10">
          <div className="w-full max-w-2xl">
            {/* Step 0: Welcome */}
            {step === 0 && (
              <div className="text-center space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
                <div className="md:hidden flex justify-center">
                  <div className="w-20 h-20 rounded-3xl bg-gradient-to-br from-indigo-500 to-violet-600 flex items-center justify-center shadow-xl shadow-indigo-500/30">
                    <Sparkles className="w-10 h-10 text-white" />
                  </div>
                </div>
                <div>
                  <h1 className="text-3xl sm:text-4xl lg:text-5xl font-black text-slate-800 mb-4">
                    Welcome to{" "}
                    <span className="bg-gradient-to-r from-indigo-600 to-violet-600 bg-clip-text text-transparent">
                      FitMaster!
                    </span>
                  </h1>
                  <p className="text-slate-400 text-lg leading-relaxed max-w-lg mx-auto">
                    Answer 7 quick questions and we'll build a personalised
                    fitness plan designed just for you.
                  </p>
                </div>
                <div className="grid grid-cols-2 sm:grid-cols-3 gap-3 max-w-lg mx-auto">
                  {steps.map((s, i) => (
                    <div
                      key={s.label}
                      className="flex items-center gap-2 px-3 py-2.5 rounded-xl bg-white border border-slate-100 shadow-sm text-left"
                    >
                      <div className="w-6 h-6 rounded-full bg-indigo-100 text-indigo-600 flex items-center justify-center text-xs font-bold shrink-0">
                        {i + 1}
                      </div>
                      <span className="text-sm font-medium text-slate-600">
                        {s.label}
                      </span>
                    </div>
                  ))}
                </div>
                <button
                  onClick={() => setStep(1)}
                  className="inline-flex items-center gap-2 px-10 py-4 rounded-2xl bg-gradient-to-r from-indigo-600 to-violet-600 text-white font-bold text-base shadow-lg shadow-indigo-500/30 hover:shadow-xl hover:shadow-indigo-500/40 transition-all duration-200 hover:-translate-y-0.5"
                >
                  Get Started
                  <ArrowRight className="w-5 h-5" />
                </button>
              </div>
            )}

            {/* Step 1: Goal */}
            {step === 1 && (
              <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-400">
                <div>
                  <p className="text-sm font-semibold text-indigo-500 uppercase tracking-wider mb-2">
                    Step 1 of 7
                  </p>
                  <h2 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-black text-slate-800">
                    What's your main goal?
                  </h2>
                  <p className="text-slate-400 mt-2">
                    This helps us focus your entire training program.
                  </p>
                </div>
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  {goals.map(({ value, label, desc, icon: Icon, gradient }) => (
                    <button
                      key={value}
                      onClick={() => set("goal", value)}
                      className={`relative flex items-start gap-4 p-5 rounded-2xl border-2 text-left transition-all duration-200 bg-white ${
                        data.goal === value
                          ? "border-transparent ring-2 ring-indigo-500 ring-offset-2 shadow-md"
                          : "border-slate-100 hover:border-slate-200 hover:shadow-sm"
                      }`}
                    >
                      <div
                        className={`w-12 h-12 rounded-xl bg-gradient-to-br ${gradient} flex items-center justify-center shadow-sm shrink-0`}
                      >
                        <Icon className="w-6 h-6 text-white" />
                      </div>
                      <div className="pt-0.5">
                        <p className="font-bold text-slate-800">{label}</p>
                        <p className="text-sm text-slate-400 mt-0.5">{desc}</p>
                      </div>
                      {data.goal === value && (
                        <CheckCircle2 className="absolute top-4 right-4 w-5 h-5 text-indigo-500" />
                      )}
                    </button>
                  ))}
                </div>
              </div>
            )}

            {/* Step 2: Fitness Level */}
            {step === 2 && (
              <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-400">
                <div>
                  <p className="text-sm font-semibold text-indigo-500 uppercase tracking-wider mb-2">
                    Step 2 of 7
                  </p>
                  <h2 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-black text-slate-800">
                    What's your fitness level?
                  </h2>
                  <p className="text-slate-400 mt-2">
                    Be honest — we'll calibrate your workouts perfectly.
                  </p>
                </div>
                <div className="space-y-3">
                  {levels.map(({ value, label, desc, years }) => (
                    <button
                      key={value}
                      onClick={() => set("fitnessLevel", value)}
                      className={`w-full flex items-center justify-between p-5 rounded-2xl border-2 text-left transition-all duration-200 bg-white ${
                        data.fitnessLevel === value
                          ? "border-indigo-500 ring-1 ring-indigo-500/20 shadow-md"
                          : "border-slate-100 hover:border-slate-200 hover:shadow-sm"
                      }`}
                    >
                      <div className="flex items-center gap-4">
                        <div
                          className={`w-12 h-12 rounded-xl flex items-center justify-center text-xl font-black transition-all ${
                            data.fitnessLevel === value
                              ? "bg-indigo-500 text-white"
                              : "bg-slate-100 text-slate-400"
                          }`}
                        >
                          {value === "BEGINNER"
                            ? "1"
                            : value === "INTERMEDIATE"
                              ? "2"
                              : "3"}
                        </div>
                        <div>
                          <p className="font-bold text-slate-800 text-base">
                            {label}
                          </p>
                          <p className="text-sm text-slate-400">
                            {desc} · {years}
                          </p>
                        </div>
                      </div>
                      {data.fitnessLevel === value && (
                        <CheckCircle2 className="w-5 h-5 text-indigo-500 shrink-0" />
                      )}
                    </button>
                  ))}
                </div>
              </div>
            )}

            {/* Step 3: Training Style */}
            {step === 3 && (
              <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-400">
                <div>
                  <p className="text-sm font-semibold text-indigo-500 uppercase tracking-wider mb-2">
                    Step 3 of 7
                  </p>
                  <h2 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-black text-slate-800">
                    How do you like to train?
                  </h2>
                  <p className="text-slate-400 mt-2">
                    Pick the style that excites you most.
                  </p>
                </div>
                <div className="space-y-3">
                  {styles.map(({ value, label, desc, icon: Icon }) => (
                    <button
                      key={value}
                      onClick={() => set("trainingStyle", value)}
                      className={`w-full flex items-center justify-between p-5 rounded-2xl border-2 text-left transition-all duration-200 bg-white ${
                        data.trainingStyle === value
                          ? "border-violet-500 ring-1 ring-violet-500/20 shadow-md"
                          : "border-slate-100 hover:border-slate-200 hover:shadow-sm"
                      }`}
                    >
                      <div className="flex items-center gap-4">
                        <div
                          className={`w-12 h-12 rounded-xl flex items-center justify-center transition-all ${
                            data.trainingStyle === value
                              ? "bg-violet-500 text-white"
                              : "bg-slate-100 text-slate-400"
                          }`}
                        >
                          <Icon className="w-6 h-6" />
                        </div>
                        <div>
                          <p className="font-bold text-slate-800 text-base">
                            {label}
                          </p>
                          <p className="text-sm text-slate-400">{desc}</p>
                        </div>
                      </div>
                      {data.trainingStyle === value && (
                        <CheckCircle2 className="w-5 h-5 text-violet-500 shrink-0" />
                      )}
                    </button>
                  ))}
                </div>
              </div>
            )}

            {/* Step 4: Split Type */}
            {step === 4 && (
              <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-400">
                <div>
                  <p className="text-sm font-semibold text-indigo-500 uppercase tracking-wider mb-2">
                    Step 4 of 7
                  </p>
                  <h2 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-black text-slate-800">
                    Choose your training split
                  </h2>
                  <p className="text-slate-400 mt-2">
                    This determines how your weekly workouts are structured.
                  </p>
                </div>
                <div className="space-y-3">
                  {splitTypes.map(({ value, label, desc, days }) => (
                    <button
                      key={value}
                      onClick={() => selectSplit(value)}
                      className={`w-full flex items-center justify-between p-5 rounded-2xl border-2 text-left transition-all duration-200 bg-white ${
                        data.splitType === value
                          ? "border-indigo-500 ring-1 ring-indigo-500/20 shadow-md"
                          : "border-slate-100 hover:border-slate-200 hover:shadow-sm"
                      }`}
                    >
                      <div className="flex items-center gap-4">
                        <div
                          className={`w-12 h-12 rounded-xl flex items-center justify-center shrink-0 transition-all ${
                            data.splitType === value
                              ? "bg-indigo-500 text-white"
                              : "bg-slate-100 text-slate-400"
                          }`}
                        >
                          <LayoutGrid className="w-5 h-5" />
                        </div>
                        <div>
                          <p className="font-bold text-slate-800 text-base">
                            {label}
                          </p>
                          <p className="text-sm text-slate-400 mt-0.5">
                            {desc}
                          </p>
                        </div>
                      </div>
                      <div className="flex items-center gap-3 shrink-0">
                        <span
                          className={`text-xs font-bold px-2.5 py-1 rounded-full ${
                            data.splitType === value
                              ? "bg-indigo-100 text-indigo-600"
                              : "bg-slate-100 text-slate-400"
                          }`}
                        >
                          {days}×/week
                        </span>
                        {data.splitType === value && (
                          <CheckCircle2 className="w-5 h-5 text-indigo-500" />
                        )}
                      </div>
                    </button>
                  ))}
                </div>
              </div>
            )}

            {/* Step 5: Body Metrics */}
            {step === 5 && (
              <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-400">
                <div>
                  <p className="text-sm font-semibold text-indigo-500 uppercase tracking-wider mb-2">
                    Step 5 of 7
                  </p>
                  <h2 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-black text-slate-800">
                    Your body metrics
                  </h2>
                  <p className="text-slate-400 mt-2">
                    Optional — but helps us personalise your plan further.
                  </p>
                </div>
                <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
                  {[
                    {
                      key: "age" as const,
                      label: "Age",
                      unit: "years",
                      icon: Calendar,
                      min: 10,
                      max: 100,
                      placeholder: "e.g. 25",
                    },
                    {
                      key: "weight" as const,
                      label: "Weight",
                      unit: "kg",
                      icon: Scale,
                      min: 30,
                      max: 300,
                      placeholder: "e.g. 75",
                    },
                    {
                      key: "height" as const,
                      label: "Height",
                      unit: "cm",
                      icon: Ruler,
                      min: 100,
                      max: 250,
                      placeholder: "e.g. 175",
                    },
                  ].map(
                    (
                      { key, label, unit, icon: Icon, min, max, placeholder },
                      idx,
                      arr,
                    ) => (
                      <div
                        key={key}
                        className={`flex items-center gap-4 px-6 py-5 ${idx < arr.length - 1 ? "border-b border-slate-50" : ""}`}
                      >
                        <div className="w-11 h-11 rounded-xl bg-slate-50 border border-slate-100 flex items-center justify-center shrink-0">
                          <Icon className="w-5 h-5 text-slate-400" />
                        </div>
                        <div className="flex-1">
                          <p className="text-sm font-bold text-slate-700">
                            {label}
                          </p>
                          <p className="text-xs text-slate-400 mt-0.5">
                            {unit}
                          </p>
                        </div>
                        <input
                          type="number"
                          min={min}
                          max={max}
                          value={data[key] ?? ""}
                          onChange={(e) =>
                            set(
                              key,
                              e.target.value
                                ? Number(e.target.value)
                                : undefined,
                            )
                          }
                          placeholder={placeholder}
                          className="w-32 px-4 py-2.5 text-right rounded-xl border border-slate-200 text-sm font-semibold text-slate-800 bg-slate-50 focus:bg-white focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100 outline-none transition-all"
                        />
                      </div>
                    ),
                  )}
                </div>
                <p className="text-xs text-slate-400 text-center">
                  You can always update these later in your profile settings.
                </p>
              </div>
            )}

            {/* Step 6: Injuries + Finish */}
            {step === 6 && (
              <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-400">
                <div>
                  <p className="text-sm font-semibold text-indigo-500 uppercase tracking-wider mb-2">
                    Step 6 of 7
                  </p>
                  <h2 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-black text-slate-800">
                    Any injuries?
                  </h2>
                  <p className="text-slate-400 mt-2">
                    We'll adjust your plan to work around them safely.
                  </p>
                </div>
                <div className="bg-white rounded-2xl border border-slate-100 shadow-sm p-6">
                  <div className="flex items-center justify-between mb-5">
                    <div className="flex items-center gap-2 text-sm font-medium text-slate-500">
                      <Shield className="w-4 h-4 text-rose-400" />
                      Select all that apply
                    </div>
                    {data.injuries.length > 0 && (
                      <span className="text-xs font-semibold px-2.5 py-1 rounded-full bg-rose-50 text-rose-500 border border-rose-100">
                        {data.injuries.length} selected
                      </span>
                    )}
                  </div>
                  <div className="grid grid-cols-3 sm:grid-cols-4 lg:grid-cols-5 gap-2">
                    {injuryOptions.map((injury) => {
                      const checked = data.injuries.includes(injury);
                      return (
                        <button
                          key={injury}
                          type="button"
                          onClick={() => toggleInjury(injury)}
                          className={`flex flex-col items-center gap-2 px-2 py-3 rounded-xl border text-xs font-semibold transition-all duration-200 ${
                            checked
                              ? "border-rose-200 bg-rose-50 text-rose-600"
                              : "border-slate-100 bg-slate-50 text-slate-500 hover:border-slate-200 hover:bg-slate-100"
                          }`}
                        >
                          <div
                            className={`w-4 h-4 rounded border-2 flex items-center justify-center ${
                              checked
                                ? "bg-rose-500 border-rose-500"
                                : "border-slate-300"
                            }`}
                          >
                            {checked && (
                              <svg
                                className="w-2.5 h-2.5 text-white"
                                fill="none"
                                viewBox="0 0 24 24"
                                stroke="currentColor"
                              >
                                <path
                                  strokeLinecap="round"
                                  strokeLinejoin="round"
                                  strokeWidth={3}
                                  d="M5 13l4 4L19 7"
                                />
                              </svg>
                            )}
                          </div>
                          {injury.replace(/_/g, " ")}
                        </button>
                      );
                    })}
                  </div>
                  <button
                    type="button"
                    onClick={() => setData((d) => ({ ...d, injuries: [] }))}
                    className="mt-5 w-full py-2.5 rounded-xl text-sm font-medium text-slate-400 hover:bg-slate-50 transition-colors"
                  >
                    No injuries — skip this
                  </button>
                </div>
              </div>
            )}

            {/* Step 7: Health Conditions & Allergies */}
            {step === 7 && (
              <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-400">
                <div>
                  <p className="text-sm font-semibold text-indigo-500 uppercase tracking-wider mb-2">
                    Step 7 of 7 · Final Step
                  </p>
                  <h2 className="text-2xl sm:text-3xl md:text-3xl lg:text-4xl font-black text-slate-800">
                    Any health conditions?
                  </h2>
                  <p className="text-slate-400 mt-2">
                    We'll adjust your nutrition plan accordingly
                  </p>
                </div>

                <div className="space-y-3">
                  {/* Diabetes */}
                  <button
                    type="button"
                    onClick={() => set("hasDiabetes", !data.hasDiabetes)}
                    className={`w-full flex items-center justify-between p-4 rounded-2xl border-2 text-left transition-all duration-200 bg-white ${
                      data.hasDiabetes
                        ? "border-rose-400 bg-rose-50 ring-1 ring-rose-400/20 shadow-sm"
                        : "border-slate-100 hover:border-slate-200 hover:shadow-sm"
                    }`}
                  >
                    <div className="flex items-center gap-4">
                      <div className={`w-10 h-10 rounded-xl flex items-center justify-center shrink-0 transition-all ${
                        data.hasDiabetes ? "bg-rose-500 text-white" : "bg-slate-100 text-slate-400"
                      }`}>
                        <Droplets className="w-5 h-5" />
                      </div>
                      <div>
                        <p className="font-bold text-slate-800 text-sm">Diabetes</p>
                        <p className="text-xs text-slate-400 mt-0.5">We'll recommend low glycemic foods</p>
                      </div>
                    </div>
                    {data.hasDiabetes && <CheckCircle2 className="w-5 h-5 text-rose-500 shrink-0" />}
                  </button>

                  {/* Heart Condition */}
                  <button
                    type="button"
                    onClick={() => set("hasHeartConditions", !data.hasHeartConditions)}
                    className={`w-full flex items-center justify-between p-4 rounded-2xl border-2 text-left transition-all duration-200 bg-white ${
                      data.hasHeartConditions
                        ? "border-rose-400 bg-rose-50 ring-1 ring-rose-400/20 shadow-sm"
                        : "border-slate-100 hover:border-slate-200 hover:shadow-sm"
                    }`}
                  >
                    <div className="flex items-center gap-4">
                      <div className={`w-10 h-10 rounded-xl flex items-center justify-center shrink-0 transition-all ${
                        data.hasHeartConditions ? "bg-rose-500 text-white" : "bg-slate-100 text-slate-400"
                      }`}>
                        <Heart className="w-5 h-5" />
                      </div>
                      <div>
                        <p className="font-bold text-slate-800 text-sm">Heart Condition</p>
                        <p className="text-xs text-slate-400 mt-0.5">We'll reduce saturated fats & sodium</p>
                      </div>
                    </div>
                    {data.hasHeartConditions && <CheckCircle2 className="w-5 h-5 text-rose-500 shrink-0" />}
                  </button>

                  {/* Hypertension */}
                  <button
                    type="button"
                    onClick={() => set("hasHypertension", !data.hasHypertension)}
                    className={`w-full flex items-center justify-between p-4 rounded-2xl border-2 text-left transition-all duration-200 bg-white ${
                      data.hasHypertension
                        ? "border-rose-400 bg-rose-50 ring-1 ring-rose-400/20 shadow-sm"
                        : "border-slate-100 hover:border-slate-200 hover:shadow-sm"
                    }`}
                  >
                    <div className="flex items-center gap-4">
                      <div className={`w-10 h-10 rounded-xl flex items-center justify-center shrink-0 transition-all ${
                        data.hasHypertension ? "bg-rose-500 text-white" : "bg-slate-100 text-slate-400"
                      }`}>
                        <ActivitySquare className="w-5 h-5" />
                      </div>
                      <div>
                        <p className="font-bold text-slate-800 text-sm">Hypertension</p>
                        <p className="text-xs text-slate-400 mt-0.5">We'll keep sodium levels low</p>
                      </div>
                    </div>
                    {data.hasHypertension && <CheckCircle2 className="w-5 h-5 text-rose-500 shrink-0" />}
                  </button>
                </div>

                <div className="bg-white rounded-2xl border border-slate-100 shadow-sm p-6 mt-6">
                  <div className="flex flex-col mb-5">
                    <h3 className="font-bold text-slate-800">Any food allergies?</h3>
                    <p className="text-xs text-slate-500 mt-1">Select all that apply</p>
                  </div>
                  <div className="grid grid-cols-2 gap-2">
                    {(["GLUTEN", "LACTOSE", "NUTS", "EGGS", "SHELLFISH", "SOY"] as AllergyType[]).map((allergy) => {
                      const checked = data.allergies.includes(allergy);
                      return (
                        <button
                          key={allergy}
                          type="button"
                          onClick={() => toggleAllergy(allergy)}
                          className={`flex items-center gap-3 px-3 py-3 rounded-xl border text-xs font-semibold transition-all duration-200 ${
                            checked
                              ? "border-rose-200 bg-rose-50 text-rose-600"
                              : "border-slate-100 bg-slate-50 text-slate-500 hover:border-slate-200 hover:bg-slate-100"
                          }`}
                        >
                          <div
                            className={`w-4 h-4 rounded border-2 flex items-center justify-center shrink-0 ${
                              checked
                                ? "bg-rose-500 border-rose-500"
                                : "border-slate-300 bg-white"
                            }`}
                          >
                            {checked && (
                              <svg className="w-2.5 h-2.5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                              </svg>
                            )}
                          </div>
                          {allergy}
                        </button>
                      );
                    })}
                  </div>

                  <button
                    type="button"
                    onClick={() => setData((d) => ({ ...d, hasDiabetes: false, hasHeartConditions: false, hasHypertension: false, allergies: [] }))}
                    className="mt-5 w-full py-2.5 rounded-xl text-sm font-medium text-slate-400 hover:bg-slate-50 transition-colors"
                  >
                    No health conditions or allergies — skip this
                  </button>
                </div>

                {error && (
                  <p className="text-sm text-red-500 text-center">{error}</p>
                )}

                <button
                  onClick={handleFinish}
                  disabled={saving}
                  className="w-full flex items-center justify-center gap-3 py-4 rounded-2xl bg-gradient-to-r from-indigo-600 to-violet-600 text-white font-bold text-base shadow-lg shadow-indigo-500/30 hover:shadow-xl hover:shadow-indigo-500/40 transition-all duration-200 hover:-translate-y-0.5 disabled:opacity-70 disabled:cursor-not-allowed disabled:transform-none"
                >
                  {saving ? (
                    <>
                      <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                      Setting up your profile...
                    </>
                  ) : (
                    <>
                      <Sparkles className="w-5 h-5" />
                      Finish & Go to Dashboard
                    </>
                  )}
                </button>
              </div>
            )}

            {/* Back / Next */}
            {step >= 1 && step < 7 && (
              <div className="mt-8 flex items-center justify-between gap-4">
                <button
                  onClick={() => setStep((s) => s - 1)}
                  className="flex items-center gap-2 px-6 py-3.5 rounded-xl border-2 border-slate-200 bg-white text-sm font-semibold text-slate-600 hover:bg-slate-50 hover:border-slate-300 transition-all"
                >
                  <ArrowLeft className="w-4 h-4" />
                  Back
                </button>
                <button
                  onClick={() => setStep((s) => s + 1)}
                  disabled={!canAdvance()}
                  className="flex items-center gap-2 px-8 py-3.5 rounded-xl bg-gradient-to-r from-indigo-600 to-violet-600 text-white text-sm font-bold shadow-md shadow-indigo-500/20 hover:shadow-lg transition-all hover:-translate-y-0.5 disabled:opacity-40 disabled:cursor-not-allowed disabled:transform-none"
                >
                  Next step
                  <ArrowRight className="w-4 h-4" />
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
