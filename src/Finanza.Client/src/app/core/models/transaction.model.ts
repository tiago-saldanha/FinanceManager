export type TransactionStatus = 'Pending' | 'Paid' | 'Cancelled';
export type TransactionType = 'Revenue' | 'Expense';

export interface Transaction {
  id: string;
  description: string;
  amount: number;
  dueDate: string;
  createdAt: string;
  paymentDate?: string;
  status: TransactionStatus;
  type: TransactionType;
  categoryName: string;
  isOverdue: boolean;
}

export interface CreateTransactionRequest {
  description: string;
  amount: number;
  dueDate: string;
  transactionType: TransactionType;
  categoryId: string;
  createdAt: string;
}

export interface PayTransactionRequest {
  paymentDate: string;
}

export interface UpdateTransactionRequest {
  description: string;
  amount: number;
  dueDate: string;
  transactionType: TransactionType;
  categoryId: string;
}

export const TRANSACTION_STATUS_LABELS: Record<TransactionStatus, string> = {
  Pending: 'Pendente',
  Paid: 'Pago',
  Cancelled: 'Cancelado',
};

export const TRANSACTION_TYPE_LABELS: Record<TransactionType, string> = {
  Revenue: 'Receita',
  Expense: 'Despesa',
};
