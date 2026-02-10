export interface EventResponseDto {
  id: number;
  title: string;
  startTime: string;    
  imageUrl?: string;
  address?: string;
  city?: string;
  categoryNames?: string[];
}
