export interface EventSubmission {
  id?: number;
  eventId?: number;
  title?: string;
  description?: string;
  startTime?: string;
  details?: string;
  submittedBy?: string;
  submittedAt?: string;
  isApproved?: boolean | null;
}
