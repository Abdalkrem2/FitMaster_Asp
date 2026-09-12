export type PaymentStatus = "PENDING" | "COMPLETED" | "FAILED" | "CANCELLED";

// Matches PaymentDto (GET /api/payments/me)
export interface Payment {
  id: number;
  packageId: number;
  packageName: string;
  amount: number;
  status: PaymentStatus | string;
  membershipId?: number;
  createdAt: string;
  completedAt?: string;
}

// Matches CheckoutSessionDto (POST /api/payments/checkout)
export interface CheckoutSession {
  paymentId: number;
  checkoutUrl: string;
}
