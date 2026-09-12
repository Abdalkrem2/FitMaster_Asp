import type { Payment, CheckoutSession } from "@/types/payment";
import { api } from "./api";

export const paymentService = {
  // Self-service: creates a Pending Payment + Stripe Checkout session for the
  // given package, returns the URL to redirect the browser to. The membership
  // itself is only ever created by the backend's webhook once Stripe confirms
  // payment - never by this call or the success-page redirect.
  createCheckoutSession: async (packageId: number): Promise<CheckoutSession> => {
    const res = await api.post("/payments/checkout", { packageId });
    return res.data;
  },

  getMyPayments: async (): Promise<Payment[]> => {
    const res = await api.get("/payments/me");
    return res.data ?? [];
  },
};
