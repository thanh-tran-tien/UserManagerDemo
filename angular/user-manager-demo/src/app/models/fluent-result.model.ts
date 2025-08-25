export interface FluentResult<T = any> {
  isSuccess: boolean;
  isFailed: boolean;
  errors?: [];
  successes?: [];
  value?: T;
  valueOrDefault: T;
}
