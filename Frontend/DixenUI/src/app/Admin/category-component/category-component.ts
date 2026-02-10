import { Component } from '@angular/core';
import { GenericService } from '../../Services/generic-service';
import { Router, RouterModule } from '@angular/router';
import { Category } from '../../Models/category';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-category-component',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './category-component.html',
  styleUrl: './category-component.css',
})
export class CategoryComponent {
  showCategoryForm  = false;
  categories:Category[]=[];
  selectedCategory: Category | null = null;
  newCategory: Category = new Category(); 

  constructor(private service: GenericService <Category>, private route:Router) { }

  ngOnInit(): void {
    console.log('CategoryComponent initialized.');
    this.loadCategories();
  }

  loadCategories(): void {
    this.service.getAll("Category").subscribe({
      next: (obj) => {
        this.categories = obj;
        console.log('Categories loaded:', obj);
      },
      error: (err) => {
        console.error('Error loading categories:', err);
      }
    });
  }

  createCategory(): void {
    this.service.post("Category", this.newCategory).subscribe({
      next: (createdCategory) => {
        console.log('Category created:', createdCategory);
        this.loadCategories(); 
        this.newCategory = new Category(); 
      },
      error: (err) => {
        console.error('Error creating category:', err);
      }
    });
  }

  editCategory(): void {
    if (this.selectedCategory) {
      const categoryDto = {
        id: this.selectedCategory.id,
        name: this.selectedCategory.name,
        imageUrl: this.selectedCategory.imageUrl
      };

      this.service.put("Category", this.selectedCategory.id, categoryDto).subscribe({
        next: (updatedCategory) => {
          console.log('Category updated:', updatedCategory);
          this.loadCategories(); 
          this.selectedCategory = null; 
        },
        error: (err) => {
          console.error('Error updating category:', err);
        }
      });
    }
  }

  deleteCategory(id: number): void {
    if (confirm('Are you sure you want to delete this category?')) {
      this.service.delete("Category", id).subscribe({
        next: () => {
          console.log('Category deleted:', id);
          this.loadCategories(); 
        },
        error: (err) => {
          console.error('Error deleting category:', err);
        }
      });
    }
  }
  selectForEdit(category: Category): void {
    this.selectedCategory = { ...category };
    this.showCategoryForm = true;
  }

    toggleCreateForm(): void {
    this.showCategoryForm = true;
    this.selectedCategory = null;
    this.newCategory = new Category();
  }
  resetForm() {
  this.selectedCategory = null;
  this.showCategoryForm = false;
  this.newCategory = new Category();
}
get categoryToEditOrCreate(): Category {
  return this.selectedCategory ?? this.newCategory;
}
  
}
