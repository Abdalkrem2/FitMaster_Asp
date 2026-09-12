import React from "react";

interface CardProps {
  children: React.ReactNode;
  className?: string;
  padding?: "none" | "sm" | "md" | "lg";
  variant?: "default" | "glass" | "gradient" | "interactive";
  hover?: boolean;
}

export const Card: React.FC<CardProps> = ({
  children,
  className = "",
  padding = "md",
  variant = "default",
  hover = false,
}) => {
  const paddings = {
    none: "",
    sm: "p-4",
    md: "p-6",
    lg: "p-8",
  };

  const variants = {
    default: "bg-white border border-slate-200/60 shadow-soft",
    glass: "glass border border-white/20 shadow-soft",
    gradient:
      "bg-gradient-to-br from-white to-slate-50/80 border border-slate-200/60 shadow-soft",
    interactive:
      "bg-white border border-slate-200/60 shadow-soft cursor-pointer",
  };

  const hoverEffect = hover
    ? "hover:shadow-medium hover:-translate-y-0.5 transition-all duration-300 ease-out"
    : "transition-shadow duration-300";

  return (
    <div
      className={`rounded-2xl ${variants[variant]} ${paddings[padding]} ${hoverEffect} ${className}`}
    >
      {children}
    </div>
  );
};

export const CardHeader: React.FC<{
  children: React.ReactNode;
  className?: string;
}> = ({ children, className = "" }) => (
  <div className={`mb-4 ${className}`}>{children}</div>
);

export const CardTitle: React.FC<{
  children: React.ReactNode;
  className?: string;
}> = ({ children, className = "" }) => (
  <h3
    className={`text-lg font-semibold text-slate-900 tracking-tight  ${className}`}
  >
    {children}
  </h3>
);
