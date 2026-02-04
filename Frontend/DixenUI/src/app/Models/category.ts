import { Evnt } from "./Evnt";

export class Category {
  id: number = 0;
  name: string = '';
  imageUrl?: string='';
  events?: Evnt[]; 
}
