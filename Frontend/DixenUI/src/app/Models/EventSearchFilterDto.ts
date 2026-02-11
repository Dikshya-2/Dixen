export interface EventSearchFilterDto {
  title?: string;         
  organizerId?: number;
  categoryIds?: number[];
  eventDate?: string;     
  venueCity?: string;
  keyword?: string;
}