import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { Question } from './question';


@Injectable({
  providedIn: 'root'
})
export class QuestionService {

  private questionsUrl = 'api/v1/questions/random';

  constructor(
    private http: HttpClient) { }

  getQuestions(): Observable<Question[]> {

    return this.http.get(this.questionsUrl).pipe(
      map(quiz => quiz['questions'])
    );
  }

}
