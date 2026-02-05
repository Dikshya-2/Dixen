import { Component } from '@angular/core';
import { Category } from '../../Models/category';
import { GenericService } from '../../Services/generic-service';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-category-list',
  imports: [CommonModule, RouterModule],
  templateUrl: './category-list.html',
  styleUrl: './category-list.css',
  standalone:true
})
export class CategoryList {
categories: Category[] = [];

  constructor(private service: GenericService<Category>,private router: Router) {}

  ngOnInit(): void {
  this.service.getAll('Category').subscribe({
    next: (data) => {
      console.log('Categories:', data); // Log data to see if it is being fetched correctly
      this.categories = data;
    },
    error: (err) => {
      console.error('Failed to load categories', err);
    }
  });
}

 goToCategory(id: number) {
    this.router.navigate(['/category', id]);
  }

  getCategoryImage(category: Category): string {
  return category.imageUrl || 'assets/images/Theater.jpg';
}
}
