export interface ApiResponseModel<T = any> {
  message: string;
  metadata: {
      data: T;
  };
  reasons?: string[];
}
