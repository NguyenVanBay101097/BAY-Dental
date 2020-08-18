import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareScenarioMessageTextareaComponent } from './tcare-scenario-message-textarea.component';

describe('TcareScenarioMessageTextareaComponent', () => {
  let component: TcareScenarioMessageTextareaComponent;
  let fixture: ComponentFixture<TcareScenarioMessageTextareaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareScenarioMessageTextareaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareScenarioMessageTextareaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
