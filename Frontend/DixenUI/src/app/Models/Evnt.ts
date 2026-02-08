import { Category } from './category';
import { Hall } from './hall';

export class Evnt {
  id: number = 0;
  title: string = '';
  description: string = '';
  startTime: Date = new Date();
  imageUrl: string = '';
  organizerId: number = 0;
  // categories!: Category[];
  categories?: Category[];
  categoryIds: number[] = [];
  halls?: Hall[];
  categoryNames?: string[];
  hallNames?: string[];
  organizerName: string = '';
  name?: string;
  address?: string;
  city?: string;
  tickets?: { id: number; type: string; price: number; quantity: number }[];
  minPrice?: number;
  maxPrice?: number;
  hallIds: number[] = [];
  date?: string;
}
