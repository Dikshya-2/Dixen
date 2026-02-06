import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CategoryComponent } from './Admin/category-component/category-component';
import { Nav } from './components/nav/nav';
import { Footercomponent } from './components/footercomponent/footercomponent';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Nav, Footercomponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'DixenUI';
}
