export interface CategoryTotal {
  received: number;
  spent: number;
  balance: number;
}

export interface Category {
  id: string;
  name: string;
  description?: string;
  total: CategoryTotal;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string;
}
