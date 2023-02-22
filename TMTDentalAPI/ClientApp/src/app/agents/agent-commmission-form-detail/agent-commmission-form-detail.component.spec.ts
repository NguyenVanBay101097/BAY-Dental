import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AgentCommmissionFormDetailComponent } from './agent-commmission-form-detail.component';

describe('AgentCommmissionFormDetailComponent', () => {
  let component: AgentCommmissionFormDetailComponent;
  let fixture: ComponentFixture<AgentCommmissionFormDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AgentCommmissionFormDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgentCommmissionFormDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
