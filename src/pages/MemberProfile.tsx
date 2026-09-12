import { useEffect, useState } from "react";
import axios from "axios";
import {
  User,
  Target,
  TrendingUp,
  Zap,
  Calendar,
  CheckCircle2,
  Save,
  AlertCircle,
  Scale,
  Ruler,
  Shield,
  LayoutGrid,
  Pencil,
  X,
  ArrowLeft,
  Droplets,
  Heart,
  ActivitySquare,
  Utensils,
  Building2,
  PersonStanding,
} from "lucide-react";
import { useAuth } from "../context/AuthContext";
import { memberService } from "../services/memberService";
import type {
  MemberProfile,
  FitnessGoal,
  FitnessLevel,
  InjuryType,
  TrainingStyle,
  SplitType,
  EquipmentPreference,
  UpdateMemberProfileRequest,
  MemberDetails,
  AllergyType,
} from "../types/member";

const fitnessGoals: {
  value: FitnessGoal;
  label: string;
  icon: typeof Target;
  color: string;
  bg: string;
}[] = [
    {
      value: "MUSCLE_GAIN",
      label: "Muscle Gain",
      icon: TrendingUp,
      color: "text-indigo-600",
      bg: "bg-indigo-50 border-indigo-200",
    },
    {
      value: "WEIGHT_LOSS",
      label: "Weight Loss",
      icon: Scale,
      color: "text-rose-600",
      bg: "bg-rose-50 border-rose-200",
    },
    {
      value: "ENDURANCE",
      label: "Endurance",
      icon: Zap,
      color: "text-amber-600",
      bg: "bg-amber-50 border-amber-200",
    },
    {
      value: "GENERAL_FITNESS",
      label: "General Fitness",
      icon: Target,
      color: "text-emerald-600",
      bg: "bg-emerald-50 border-emerald-200",
    },
  ];

const fitnessLevels: { value: FitnessLevel; label: string; desc: string }[] = [
  { value: "BEGINNER", label: "Beginner", desc: "0–1 year" },
  { value: "INTERMEDIATE", label: "Intermediate", desc: "1–3 years" },
  { value: "ADVANCED", label: "Advanced", desc: "3+ years" },
];

const trainingStyles: { value: TrainingStyle; label: string; desc: string }[] =
  [
    { value: "STRENGTH", label: "Strength", desc: "Heavy compound lifts" },
    { value: "HYPERTROPHY", label: "Hypertrophy", desc: "Muscle building" },
    { value: "CIRCUIT", label: "Circuit", desc: "High intensity" },
  ];

const equipmentOptions: {
  value: EquipmentPreference;
  label: string;
  desc: string;
  icon: typeof Building2;
}[] = [
  {
    value: "GYM",
    label: "Gym Equipment",
    desc: "Barbells, machines, dumbbells & more",
    icon: Building2,
  },
  {
    value: "BODYWEIGHT",
    label: "Bodyweight Only",
    desc: "No equipment — train anywhere",
    icon: PersonStanding,
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

const MemberProfile = () => {
  const { user } = useAuth();
  const [profile, setProfile] = useState<MemberProfile | null>(null);
  const [memberDetails, setMemberDetails] = useState<MemberDetails | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    const loadProfile = async () => {
      try {
        const detailsData = await memberService.getMyDetails();
        setMemberDetails(detailsData);

        const data = await memberService.getMyProfile();
        if (data) {
          setProfile(data);
        } else {
          setProfile({
            goal: "GENERAL_FITNESS",
            fitnessLevel: "BEGINNER",
            injuries: [],
            weight: undefined,
            height: undefined,
            age: undefined,
            trainingStyle: "STRENGTH",
            equipmentPreference: "GYM",
            hasDiabetes: false,
            hasHeartConditions: false,
            hasHypertension: false,
            allergies: [],
          });
        }
      } catch (err) {
        if (axios.isAxiosError(err)) {
          const status = err.response?.status;
          if (status === 401 || status === 403) {
            setError("You must be signed in as a member to view this page.");
          } else {
            setError(
              err.response?.data?.message ??
              "Unable to load profile. Please try again.",
            );
          }
        } else {
          setError("Unable to load profile. Please try again.");
        }
      } finally {
        setIsLoading(false);
      }
    };
    loadProfile();
  }, []);

  const handleChange = <K extends keyof MemberProfile>(
    key: K,
    value: MemberProfile[K],
  ) => {
    setProfile((current) => (current ? { ...current, [key]: value } : current));
  };

  const handleSplitSelect = (value: SplitType) => {
    setProfile((current) =>
      current
        ? { ...current, splitType: value, daysPerWeek: splitDays[value] }
        : current,
    );
  };


  const handleInjuriesToggle = (injury: InjuryType) => {
    setProfile((current) => {
      if (!current) return current;
      const injuries = current.injuries ?? [];
      const hasInjury = injuries.includes(injury);
      return {
        ...current,
        injuries: hasInjury
          ? injuries.filter((item) => item !== injury)
          : [...injuries, injury],
      };
    });
  };

  const handleAllergyToggle = (allergy: AllergyType) => {
    setProfile((current) => {
      if (!current) return current;
      const allergies = current.allergies ?? [];
      const hasAllergy = allergies.includes(allergy);
      return {
        ...current,
        allergies: hasAllergy
          ? allergies.filter((item) => item !== allergy)
          : [...allergies, allergy],
      };
    });
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!profile) return;
    setIsSaving(true);
    setError(null);
    setSuccess(null);
    const request: UpdateMemberProfileRequest = {
      goal: profile.goal,
      fitnessLevel: profile.fitnessLevel,
      splitType: profile.splitType ?? "FULL_BODY",
      equipmentPreference: profile.equipmentPreference ?? "GYM",
      injuries:
        profile.injuries && profile.injuries.length > 0
          ? profile.injuries
          : null,
      weight: profile.weight ?? null,
      height: profile.height ?? null,
      age: profile.age ?? null,
      trainingStyle: profile.trainingStyle,
      hasDiabetes: profile.hasDiabetes,
      hasHeartConditions: profile.hasHeartConditions,
      hasHypertension: profile.hasHypertension,
      allergies: profile.allergies,
    };
    try {
      await memberService.updateMyProfile(request);
      setSuccess("Profile saved successfully.");
      setIsEditing(false);
    } catch (err) {
      setError("Unable to save profile. Please try again.");
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center text-slate-400">
        Loading profile...
      </div>
    );
  }

  const renderReadOnlyView = () => {
    const goalInfo = fitnessGoals.find(g => g.value === profile?.goal);
    const GoalIcon = goalInfo?.icon || Target;

    const levelInfo = fitnessLevels.find(l => l.value === profile?.fitnessLevel);
    const styleInfo = trainingStyles.find(s => s.value === profile?.trainingStyle);
    const splitInfo = splitTypes.find(s => s.value === profile?.splitType);
    const equipmentInfo = equipmentOptions.find(e => e.value === profile?.equipmentPreference);
    const EquipmentIcon = equipmentInfo?.icon || Building2;

    return (
      <div className="space-y-6 animate-in fade-in zoom-in duration-300">
        {/* Profile Header */}
        <div className="bg-white rounded-3xl border border-slate-100 shadow-sm p-8 text-center flex flex-col items-center">
          <div className="w-24 h-24 rounded-full bg-gradient-to-br from-indigo-500 to-violet-600 flex items-center justify-center shadow-lg shadow-indigo-500/30 mb-4 overflow-hidden border-4 border-white">
            {memberDetails?.profilePicture ? (
              <img src={memberDetails.profilePicture} alt="Profile" className="w-full h-full object-cover" />
            ) : (
              <User className="w-10 h-10 text-white" />
            )}
          </div>
          <h2 className="text-2xl font-bold text-slate-800">{user?.name || memberDetails?.fullName || "Member Name"}</h2>
          <p className="text-slate-500 text-sm mt-1 mb-6">{user?.phone || memberDetails?.phone || "Active Member"}</p>
          <button
            onClick={() => setIsEditing(true)}
            className="flex items-center gap-2 px-6 py-2.5 rounded-xl bg-slate-50 text-slate-700 font-medium border border-slate-200 hover:bg-slate-100 transition-colors"
          >
            <Pencil className="w-4 h-4" />
            Edit Profile
          </button>
        </div>

        {/* Stats Grid */}
        <div className="grid grid-cols-2 lg:grid-cols-3 gap-4">
          {/* Body Metrics */}
          <div className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm flex flex-col items-center justify-center gap-2 text-center">
            <Scale className="w-6 h-6 text-indigo-500 mb-1" />
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Weight</p>
            <p className="text-xl sm:text-2xl font-bold text-slate-800">{profile?.weight ? `${profile.weight} kg` : "--"}</p>
          </div>
          <div className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm flex flex-col items-center justify-center gap-2 text-center">
            <Ruler className="w-6 h-6 text-emerald-500 mb-1" />
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Height</p>
            <p className="text-xl sm:text-2xl font-bold text-slate-800">{profile?.height ? `${profile.height} cm` : "--"}</p>
          </div>
          <div className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm flex flex-col items-center justify-center gap-2 text-center col-span-2 lg:col-span-1">
            <Calendar className="w-6 h-6 text-amber-500 mb-1" />
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Age</p>
            <p className="text-xl sm:text-2xl font-bold text-slate-800">{profile?.age ? `${profile.age} yrs` : "--"}</p>
          </div>

          {/* Goal */}
          <div className={`p-6 rounded-2xl border ${goalInfo ? goalInfo.bg : "bg-white border-slate-100"} shadow-sm flex flex-col items-center justify-center gap-2 text-center col-span-2 lg:col-span-1`}>
            <GoalIcon className={`w-6 h-6 ${goalInfo ? goalInfo.color : "text-slate-400"} mb-1`} />
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Goal</p>
            <p className={`text-lg sm:text-xl font-bold ${goalInfo ? goalInfo.color : "text-slate-800"}`}>
              {goalInfo?.label || "Not Set"}
            </p>
          </div>

          {/* Fitness Level */}
          <div className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm flex flex-col items-center justify-center gap-2 text-center">
            <TrendingUp className="w-6 h-6 text-violet-500 mb-1" />
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Level</p>
            <p className="text-lg sm:text-xl font-bold text-slate-800">{levelInfo?.label || "Not Set"}</p>
          </div>

          {/* Training Style */}
          <div className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm flex flex-col items-center justify-center gap-2 text-center">
            <Zap className="w-6 h-6 text-rose-500 mb-1" />
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Style</p>
            <p className="text-lg sm:text-xl font-bold text-slate-800">{styleInfo?.label || "Not Set"}</p>
          </div>

          {/* Training Split */}
          <div className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm flex flex-col items-center justify-center gap-2 text-center">
            <LayoutGrid className="w-6 h-6 text-sky-500 mb-1" />
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Training Split</p>
            <p className="text-lg sm:text-xl font-bold text-slate-800">{splitInfo?.label || "Not Set"}</p>
            {splitInfo && <p className="text-sm text-slate-500">{splitInfo.desc}</p>}
          </div>

          {/* Equipment Preference */}
          <div className="col-span-2 lg:col-span-2 bg-white p-6 rounded-2xl border border-slate-100 shadow-sm flex flex-col items-center justify-center gap-2 text-center">
            <EquipmentIcon className="w-6 h-6 text-indigo-500 mb-1" />
            <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Equipment</p>
            <p className="text-lg sm:text-xl font-bold text-slate-800">{equipmentInfo?.label || "Not Set"}</p>
          </div>

          {/* Injuries */}
          {profile?.injuries && profile.injuries.length > 0 && (
            <div className="col-span-2 lg:col-span-3 bg-white p-6 rounded-2xl border border-rose-100 shadow-sm flex flex-col items-center gap-3 text-center">
              <div className="flex items-center gap-2">
                <Shield className="w-5 h-5 text-rose-500" />
                <p className="text-xs font-semibold text-rose-500 uppercase tracking-wider">Reported Injuries</p>
              </div>
              <div className="flex flex-wrap justify-center gap-2">
                {profile.injuries.map(inj => (
                  <span key={inj} className="px-3 py-1 bg-rose-50 text-rose-600 rounded-full text-xs sm:text-sm font-medium">
                    {inj.replace(/_/g, " ")}
                  </span>
                ))}
              </div>
            </div>
          )}

          {/* Health Conditions & Allergies */}
          {(profile?.hasDiabetes || profile?.hasHeartConditions || profile?.hasHypertension || (profile?.allergies && profile.allergies.length > 0)) && (
            <div className="col-span-2 lg:col-span-3 bg-white p-6 rounded-2xl border border-slate-100 shadow-sm space-y-4">
              <div className="flex items-center gap-2">
                <Utensils className="w-5 h-5 text-indigo-500" />
                <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider">Health & Nutrition</p>
              </div>
              
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                {/* Conditions */}
                <div className="space-y-2">
                  <p className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">Medical Conditions</p>
                  <div className="flex flex-wrap gap-2">
                    {profile?.hasDiabetes && (
                      <span className="flex items-center gap-1.5 px-3 py-1 bg-rose-50 text-rose-600 rounded-full text-xs font-bold border border-rose-100">
                        <Droplets className="w-3 h-3" /> Diabetes
                      </span>
                    )}
                    {profile?.hasHeartConditions && (
                      <span className="flex items-center gap-1.5 px-3 py-1 bg-rose-50 text-rose-600 rounded-full text-xs font-bold border border-rose-100">
                        <Heart className="w-3 h-3" /> Heart Condition
                      </span>
                    )}
                    {profile?.hasHypertension && (
                      <span className="flex items-center gap-1.5 px-3 py-1 bg-rose-50 text-rose-600 rounded-full text-xs font-bold border border-rose-100">
                        <ActivitySquare className="w-3 h-3" /> Hypertension
                      </span>
                    )}
                    {!profile?.hasDiabetes && !profile?.hasHeartConditions && !profile?.hasHypertension && (
                      <span className="text-xs text-slate-400">No medical conditions reported</span>
                    )}
                  </div>
                </div>

                {/* Allergies */}
                <div className="space-y-2">
                  <p className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">Food Allergies</p>
                  <div className="flex flex-wrap gap-2">
                    {profile?.allergies && profile.allergies.length > 0 ? (
                      profile.allergies.map(allergy => (
                        <span key={allergy} className="px-3 py-1 bg-amber-50 text-amber-700 rounded-full text-xs font-bold border border-amber-100">
                          {allergy}
                        </span>
                      ))
                    ) : (
                      <span className="text-xs text-slate-400">No food allergies reported</span>
                    )}
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    );
  };

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500">

      {isEditing ? (
        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="flex items-center gap-3">
            <button
              type="button"
              onClick={() => setIsEditing(false)}
              className="p-2 -ml-2 rounded-xl text-slate-400 hover:text-slate-600 hover:bg-slate-100 transition-colors"
            >
              <ArrowLeft className="w-5 h-5" />
            </button>
            <h2 className="text-lg font-bold text-slate-800">Edit Profile Details</h2>
          </div>
          {/* ─── Feedback Messages ─── */}
          {error && (
            <div className="flex items-center gap-3 px-4 py-3 rounded-xl bg-red-50 border border-red-100 text-red-700 text-sm">
              <AlertCircle className="w-4 h-4 shrink-0" />
              {error}
            </div>
          )}
          {success && (
            <div className="flex items-center gap-3 px-4 py-3 rounded-xl bg-emerald-50 border border-emerald-100 text-emerald-700 text-sm">
              <CheckCircle2 className="w-4 h-4 shrink-0" />
              {success}
            </div>
          )}

          {/* ─── Body Metrics ─── */}
          <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
            <div className="px-6 py-4 border-b border-slate-50 flex items-center gap-2">
              <div className="w-7 h-7 rounded-lg bg-slate-50 flex items-center justify-center">
                <Ruler className="w-3.5 h-3.5 text-slate-500" />
              </div>
              <h2 className="text-sm font-semibold text-slate-700">
                Body Metrics
              </h2>
            </div>
            <div className="p-6 grid grid-cols-1 sm:grid-cols-3 gap-4">
              {[
                {
                  key: "weight" as const,
                  label: "Weight",
                  unit: "kg",
                  icon: Scale,
                  min: 30,
                  max: 300,
                },
                {
                  key: "height" as const,
                  label: "Height",
                  unit: "cm",
                  icon: Ruler,
                  min: 100,
                  max: 250,
                },
                {
                  key: "age" as const,
                  label: "Age",
                  unit: "years",
                  icon: Calendar,
                  min: 10,
                  max: 100,
                },
              ].map(({ key, label, unit, icon: Icon, min, max }) => (
                <div key={key}>
                  <label className="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2">
                    {label}
                  </label>
                  <div className="relative">
                    <div className="absolute left-3 top-1/2 -translate-y-1/2">
                      <Icon className="w-4 h-4 text-slate-300" />
                    </div>
                    <input
                      type="number"
                      min={min}
                      max={max}
                      value={profile?.[key] ?? ""}
                      onChange={(e) =>
                        handleChange(
                          key,
                          e.target.value ? Number(e.target.value) : undefined,
                        )
                      }
                      className="w-full pl-10 pr-14 py-3 rounded-xl border border-slate-200 text-sm text-slate-800 font-medium bg-slate-50 focus:bg-white focus:border-indigo-400 focus:ring-2 focus:ring-indigo-100 outline-none transition-all"
                      placeholder={`Enter ${label.toLowerCase()}`}
                    />
                    <span className="absolute right-3 top-1/2 -translate-y-1/2 text-xs text-slate-400 font-medium">
                      {unit}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* ─── Fitness Goal ─── */}
          <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
            <div className="px-6 py-4 border-b border-slate-50 flex items-center gap-2">
              <div className="w-7 h-7 rounded-lg bg-indigo-50 flex items-center justify-center">
                <Target className="w-3.5 h-3.5 text-indigo-500" />
              </div>
              <h2 className="text-sm font-semibold text-slate-700">
                Fitness Goal
              </h2>
            </div>
            <div className="p-6 grid grid-cols-1 sm:grid-cols-2 gap-3">
              {fitnessGoals.map(({ value, label, icon: Icon, color, bg }) => (
                <button
                  key={value}
                  type="button"
                  onClick={() => handleChange("goal", value)}
                  className={`flex items-center gap-3 p-4 rounded-xl border-2 transition-all duration-200 text-left ${profile?.goal === value
                    ? `${bg} ${color} border-current shadow-sm`
                    : "border-slate-100 hover:border-slate-200 text-slate-600 hover:bg-slate-50"
                    }`}
                >
                  <div
                    className={`w-9 h-9 rounded-xl flex items-center justify-center shrink-0 ${profile?.goal === value ? "bg-white/60" : "bg-slate-100"
                      }`}
                  >
                    <Icon
                      className={`w-4 h-4 ${profile?.goal === value ? color : "text-slate-400"}`}
                    />
                  </div>
                  <span className="text-sm font-semibold">{label}</span>
                  {profile?.goal === value && (
                    <CheckCircle2 className={`w-4 h-4 ml-auto ${color}`} />
                  )}
                </button>
              ))}
            </div>
          </div>

          {/* ─── Fitness Level + Training Style ─── */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
            {/* Fitness Level */}
            <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
              <div className="px-5 py-4 border-b border-slate-50 flex items-center gap-2">
                <div className="w-7 h-7 rounded-lg bg-violet-50 flex items-center justify-center">
                  <TrendingUp className="w-3.5 h-3.5 text-violet-500" />
                </div>
                <h2 className="text-sm font-semibold text-slate-700">
                  Fitness Level
                </h2>
              </div>
              <div className="p-5 space-y-2">
                {fitnessLevels.map(({ value, label, desc }) => (
                  <button
                    key={value}
                    type="button"
                    onClick={() => handleChange("fitnessLevel", value)}
                    className={`w-full flex items-center justify-between p-3 rounded-xl border transition-all duration-200 ${profile?.fitnessLevel === value
                      ? "border-violet-200 bg-violet-50 text-violet-700"
                      : "border-slate-100 hover:border-slate-200 hover:bg-slate-50 text-slate-600"
                      }`}
                  >
                    <div className="flex items-center gap-3 text-left">
                      <div
                        className={`w-2 h-2 rounded-full ${profile?.fitnessLevel === value
                          ? "bg-violet-500"
                          : "bg-slate-300"
                          }`}
                      />
                      <div>
                        <p className="text-sm font-semibold">{label}</p>
                        <p className="text-xs opacity-60">{desc}</p>
                      </div>
                    </div>
                    {profile?.fitnessLevel === value && (
                      <CheckCircle2 className="w-4 h-4 text-violet-500" />
                    )}
                  </button>
                ))}
              </div>
            </div>

            {/* Training Style */}
            <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
              <div className="px-5 py-4 border-b border-slate-50 flex items-center gap-2">
                <div className="w-7 h-7 rounded-lg bg-amber-50 flex items-center justify-center">
                  <Zap className="w-3.5 h-3.5 text-amber-500" />
                </div>
                <h2 className="text-sm font-semibold text-slate-700">
                  Training Style
                </h2>
              </div>
              <div className="p-5 space-y-2">
                {trainingStyles.map(({ value, label, desc }) => (
                  <button
                    key={value}
                    type="button"
                    onClick={() => handleChange("trainingStyle", value)}
                    className={`w-full flex items-center justify-between p-3 rounded-xl border transition-all duration-200 ${profile?.trainingStyle === value
                      ? "border-amber-200 bg-amber-50 text-amber-700"
                      : "border-slate-100 hover:border-slate-200 hover:bg-slate-50 text-slate-600"
                      }`}
                  >
                    <div className="flex items-center gap-3 text-left">
                      <div
                        className={`w-2 h-2 rounded-full ${profile?.trainingStyle === value
                          ? "bg-amber-500"
                          : "bg-slate-300"
                          }`}
                      />
                      <div>
                        <p className="text-sm font-semibold">{label}</p>
                        <p className="text-xs opacity-60">{desc}</p>
                      </div>
                    </div>
                    {profile?.trainingStyle === value && (
                      <CheckCircle2 className="w-4 h-4 text-amber-500" />
                    )}
                  </button>
                ))}
              </div>
            </div>
          </div>

          {/* ─── Training Split ─── */}
          <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
            <div className="px-6 py-4 border-b border-slate-50 flex items-center gap-2">
              <div className="w-7 h-7 rounded-lg bg-emerald-50 flex items-center justify-center">
                <LayoutGrid className="w-3.5 h-3.5 text-emerald-500" />
              </div>
              <h2 className="text-sm font-semibold text-slate-700">
                Training Split
              </h2>
            </div>
            <div className="p-6 space-y-3">
              {splitTypes.map(({ value, label, desc, days }) => (
                <button
                  key={value}
                  type="button"
                  onClick={() => handleSplitSelect(value)}
                  className={`w-full flex items-center justify-between p-4 rounded-2xl border-2 text-left transition-all duration-200 bg-white ${profile?.splitType === value
                    ? "border-indigo-500 ring-1 ring-indigo-500/20 shadow-md"
                    : "border-slate-100 hover:border-slate-200 hover:shadow-sm"
                    }`}
                >
                  <div className="flex items-center gap-3">
                    <div
                      className={`w-10 h-10 rounded-xl flex items-center justify-center shrink-0 transition-all ${profile?.splitType === value
                        ? "bg-indigo-500 text-white"
                        : "bg-slate-100 text-slate-400"
                        }`}
                    >
                      <LayoutGrid className="w-4 h-4" />
                    </div>
                    <div>
                      <p className="font-semibold text-slate-800 text-sm">
                        {label}
                      </p>
                      <p className="text-xs text-slate-400 mt-0.5">{desc}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2 shrink-0">
                    <span
                      className={`text-xs font-bold px-2 py-0.5 rounded-full ${profile?.splitType === value
                        ? "bg-indigo-100 text-indigo-600"
                        : "bg-slate-100 text-slate-400"
                        }`}
                    >
                      {days}×/wk
                    </span>
                    {profile?.splitType === value && (
                      <CheckCircle2 className="w-4 h-4 text-indigo-500" />
                    )}
                  </div>
                </button>
              ))}
            </div>
          </div>

          {/* ─── Equipment Preference ─── */}
          <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
            <div className="px-6 py-4 border-b border-slate-50 flex items-center gap-2">
              <div className="w-7 h-7 rounded-lg bg-sky-50 flex items-center justify-center">
                <Building2 className="w-3.5 h-3.5 text-sky-500" />
              </div>
              <h2 className="text-sm font-semibold text-slate-700">
                Equipment Preference
              </h2>
            </div>
            <div className="p-6 space-y-3">
              {equipmentOptions.map(({ value, label, desc, icon: Icon }) => (
                <button
                  key={value}
                  type="button"
                  onClick={() => handleChange("equipmentPreference", value)}
                  className={`w-full flex items-center justify-between p-4 rounded-2xl border-2 text-left transition-all duration-200 bg-white ${profile?.equipmentPreference === value
                    ? "border-indigo-500 ring-1 ring-indigo-500/20 shadow-md"
                    : "border-slate-100 hover:border-slate-200 hover:shadow-sm"
                    }`}
                >
                  <div className="flex items-center gap-3">
                    <div
                      className={`w-10 h-10 rounded-xl flex items-center justify-center shrink-0 transition-all ${profile?.equipmentPreference === value
                        ? "bg-indigo-500 text-white"
                        : "bg-slate-100 text-slate-400"
                        }`}
                    >
                      <Icon className="w-4 h-4" />
                    </div>
                    <div>
                      <p className="font-semibold text-slate-800 text-sm">
                        {label}
                      </p>
                      <p className="text-xs text-slate-400 mt-0.5">{desc}</p>
                    </div>
                  </div>
                  {profile?.equipmentPreference === value && (
                    <CheckCircle2 className="w-4 h-4 text-indigo-500 shrink-0" />
                  )}
                </button>
              ))}
            </div>
          </div>

          {/* ─── Injuries ─── */}
          <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
            <div className="px-6 py-4 border-b border-slate-50 flex items-center justify-between">
              <div className="flex items-center gap-2">
                <div className="w-7 h-7 rounded-lg bg-rose-50 flex items-center justify-center">
                  <Shield className="w-3.5 h-3.5 text-rose-400" />
                </div>
                <h2 className="text-sm font-semibold text-slate-700">
                  Injury History
                </h2>
              </div>
              {(profile?.injuries?.length ?? 0) > 0 && (
                <span className="text-xs font-medium px-2.5 py-1 rounded-full bg-rose-50 text-rose-500">
                  {profile?.injuries?.length} selected
                </span>
              )}
            </div>
            <div className="p-6">
              <p className="text-xs text-slate-400 mb-4">
                Select any current or past injuries so we can customize your plan.
              </p>
              <div className="grid grid-cols-2 sm:grid-cols-3 gap-2.5">
                {injuryOptions.map((injury) => {
                  const checked = profile?.injuries?.includes(injury) ?? false;
                  return (
                    <button
                      key={injury}
                      type="button"
                      onClick={() => handleInjuriesToggle(injury)}
                      className={`flex items-center gap-2 px-3 py-2.5 rounded-xl border text-sm font-medium transition-all duration-200 ${checked
                        ? "border-rose-200 bg-rose-50 text-rose-600"
                        : "border-slate-100 bg-slate-50 text-slate-500 hover:border-slate-200 hover:bg-slate-100"
                        }`}
                    >
                      <div
                        className={`w-3.5 h-3.5 rounded border flex items-center justify-center shrink-0 transition-all ${checked
                          ? "bg-rose-500 border-rose-500"
                          : "border-slate-300"
                          }`}
                      >
                        {checked && (
                          <svg
                            className="w-2 h-2 text-white"
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
                      <span>{injury.replace(/_/g, " ")}</span>
                    </button>
                  );
                })}
              </div>
            </div>
          </div>

          {/* ─── Health Conditions ─── */}
          <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
            <div className="px-6 py-4 border-b border-slate-50 flex items-center gap-2">
              <div className="w-7 h-7 rounded-lg bg-rose-50 flex items-center justify-center">
                <Shield className="w-3.5 h-3.5 text-rose-500" />
              </div>
              <h2 className="text-sm font-semibold text-slate-700">
                Health Conditions
              </h2>
            </div>
            <div className="p-6 space-y-3">
              {[
                { key: "hasDiabetes" as const, label: "Diabetes", icon: Droplets, sub: "Low glycemic focus" },
                { key: "hasHeartConditions" as const, label: "Heart Condition", icon: Heart, sub: "Heart-healthy fats" },
                { key: "hasHypertension" as const, label: "Hypertension", icon: ActivitySquare, sub: "Sodium-controlled" },
              ].map(({ key, label, icon: Icon, sub }) => (
                <button
                  key={key}
                  type="button"
                  onClick={() => handleChange(key, !profile?.[key])}
                  className={`w-full flex items-center justify-between p-4 rounded-2xl border-2 text-left transition-all duration-200 bg-white ${
                    profile?.[key]
                      ? "border-rose-400 bg-rose-50 ring-1 ring-rose-400/20"
                      : "border-slate-100 hover:border-slate-200"
                  }`}
                >
                  <div className="flex items-center gap-4">
                    <div className={`w-10 h-10 rounded-xl flex items-center justify-center shrink-0 transition-all ${
                      profile?.[key] ? "bg-rose-500 text-white" : "bg-slate-100 text-slate-400"
                    }`}>
                      <Icon className="w-5 h-5" />
                    </div>
                    <div>
                      <p className="font-bold text-slate-800 text-sm">{label}</p>
                      <p className="text-xs text-slate-400 mt-0.5">{sub}</p>
                    </div>
                  </div>
                  {profile?.[key] && <CheckCircle2 className="w-5 h-5 text-rose-500 shrink-0" />}
                </button>
              ))}
            </div>
          </div>

          {/* ─── Food Allergies ─── */}
          <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
            <div className="px-6 py-4 border-b border-slate-50 flex items-center gap-2">
              <div className="w-7 h-7 rounded-lg bg-amber-50 flex items-center justify-center">
                <Utensils className="w-3.5 h-3.5 text-amber-500" />
              </div>
              <h2 className="text-sm font-semibold text-slate-700">
                Food Allergies
              </h2>
            </div>
            <div className="p-6">
              <div className="grid grid-cols-2 sm:grid-cols-3 gap-2.5">
                {(["GLUTEN", "LACTOSE", "NUTS", "EGGS", "SHELLFISH", "SOY"] as AllergyType[]).map((allergy) => {
                  const checked = profile?.allergies?.includes(allergy) ?? false;
                  return (
                    <button
                      key={allergy}
                      type="button"
                      onClick={() => handleAllergyToggle(allergy)}
                      className={`flex items-center gap-2 px-3 py-2.5 rounded-xl border text-sm font-medium transition-all duration-200 ${checked
                        ? "border-amber-200 bg-amber-50 text-amber-700"
                        : "border-slate-100 bg-slate-50 text-slate-500 hover:border-slate-200 hover:bg-slate-100"
                        }`}
                    >
                      <div
                        className={`w-3.5 h-3.5 rounded border flex items-center justify-center shrink-0 transition-all ${checked
                          ? "bg-amber-500 border-amber-500"
                          : "border-slate-300 bg-white"
                          }`}
                      >
                        {checked && (
                          <svg className="w-2 h-2 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                          </svg>
                        )}
                      </div>
                      <span>{allergy}</span>
                    </button>
                  );
                })}
              </div>
            </div>
          </div>

          {/* ─── Save Button ─── */}
          <div className="flex justify-end">
            <button
              type="submit"
              disabled={isSaving}
              className="flex items-center gap-2.5 px-8 py-3 rounded-2xl bg-gradient-to-r from-indigo-600 to-violet-600 text-white text-sm font-semibold shadow-md shadow-indigo-500/25 hover:shadow-lg hover:shadow-indigo-500/30 transition-all duration-200 hover:-translate-y-0.5 disabled:opacity-60 disabled:cursor-not-allowed disabled:transform-none"
            >
              {isSaving ? (
                <>
                  <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                  Saving...
                </>
              ) : (
                <>
                  <Save className="w-4 h-4" />
                  Save Profile
                </>
              )}
            </button>
          </div>
        </form>
      ) : (
        renderReadOnlyView()
      )}
    </div>
  );
};

export default MemberProfile;
