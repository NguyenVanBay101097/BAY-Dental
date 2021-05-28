import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AgentCommissionFormDetailComponent } from './agent-commission-form-detail.component';

describe('AgentCommissionFormDetailComponent', () => {
  let component: AgentCommissionFormDetailComponent;
  let fixture: ComponentFixture<AgentCommissionFormDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AgentCommissionFormDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgentCommissionFormDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
