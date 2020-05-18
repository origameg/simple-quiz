import { Component } from '@angular/core';
import { Quiz } from './quiz';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  
  title = 'Simple Quiz';
  quiz:Quiz;

  constructor()
  {
    this.quiz = {};
  }
}
