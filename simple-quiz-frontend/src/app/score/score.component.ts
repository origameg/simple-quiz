import { Component, OnInit, Input } from '@angular/core';
import { Quiz } from '../quiz';
import { Score } from '../score';


@Component({
  selector: 'app-score',
  templateUrl: './score.component.html',
  styleUrls: ['./score.component.css']
})
export class ScoreComponent implements OnInit {

  @Input() quiz: Quiz;

  constructor() { }

  ngOnInit(): void {
  }

}
