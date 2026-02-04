import { Component, HostListener, inject, OnInit } from '@angular/core';
import { Authservice } from '../../Services/authservice';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-nav',
  imports: [CommonModule, FormsModule, RouterModule, TranslateModule],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav implements OnInit{
  searchQuery: string = '';


 // Dropdown state
  showDropdown = false;
  navOpen = false;
  currentLang = 'en';
  currentFlag = 'https://flagcdn.com/w20/us.png';


  private translate = inject(TranslateService);

  constructor(public authService: Authservice) {
     console.log('NavComponent loaded');
  console.log('Current lang:', this.translate.currentLang);
  console.log('Available langs:', this.translate.langs);
  }
     ngOnInit() {
    console.log('NavComponent ngOnInit');
    this.initializeLanguage();
  }
  
  private initializeLanguage() {
  console.log('🔧 initializeLanguage START');
  this.translate.addLangs(['en', 'da']);
  this.translate.setDefaultLang('da');
  const browserLang = this.translate.getBrowserLang();
  console.log('Browser lang:', browserLang);
  
  const selectedLang = browserLang?.match(/en|da/) ? browserLang : 'da';
  console.log('Selected lang:', selectedLang);
  
  this.translate.use(selectedLang).subscribe(() => {
    console.log('✅ Language LOADED:', this.translate.currentLang);
    this.updateFlag(this.translate.currentLang || 'en');
  });
}

 
  
  // private initializeLanguage() {
  //   this.translate.addLangs(['en', 'da']);
  //   this.translate.setDefaultLang('da');
  //   const browserLang = this.translate.getBrowserLang();
  //   const selectedLang = browserLang?.match(/en|da/) ? browserLang : 'da';
  //   this.translate.use(selectedLang).subscribe(() => {
  //   this.updateFlag(this.translate.currentLang || 'en');
  // });
  // }

  onSearch(event: Event) {
    event.preventDefault();
    if (this.searchQuery.trim()) {
      console.log('Search query:', this.searchQuery);
      // Implement your search logic here
      // Example: this.router.navigate(['/search'], { queryParams: { q: this.searchQuery } });
    }
  }

  setLanguage(lang: string) {
    this.updateFlag(lang);
    this.translate.use(lang);
    this.showDropdown = false; // close menu after selection
  }

   toggleDropdown(event: Event) {
    event.preventDefault();
    event.stopPropagation();
    this.showDropdown = !this.showDropdown;
  }

  private updateFlag(lang: string) {
    this.currentLang = lang;
    this.currentFlag =
      lang === 'da'
        ? 'assets/icons/denmark-flag-icon.svg'
        : 'assets/icons/united-states-flag-icon.svg';
  }
   // Close dropdown when clicking outside
  @HostListener('document:click', ['$event'])
  closeDropdownOnClickOutside(event: Event) {
    const target = event.target as HTMLElement;
    const dropdown = target.closest('.language-dropdown');
    
    if (!dropdown && this.showDropdown) {
      this.showDropdown = false;
    }
  }

  // Handle window scroll for navbar effects
  @HostListener('window:scroll', ['$event'])
  onWindowScroll(event: Event) {
    const navbar = document.querySelector('.modern-navbar') as HTMLElement;
    if (navbar) {
      if (window.pageYOffset > 50) {
        navbar.style.background = 'rgba(102, 126, 234, 0.95)';
        navbar.style.backdropFilter = 'blur(15px)';
      } else {
        navbar.style.background = 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)';
        navbar.style.backdropFilter = 'blur(10px)';
      }
    }
  }

}
function ngOnInit() {
  throw new Error('Function not implemented.');
}

