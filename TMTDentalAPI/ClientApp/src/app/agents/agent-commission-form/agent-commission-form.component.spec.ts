import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AgentCommissionFormComponent } from './agent-commission-form.component';

describe('AgentCommissionFormComponent', () => {
  let component: AgentCommissionFormComponent;
  let fixture: ComponentFixture<AgentCommissionFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AgentCommissionFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgentCommissionFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
