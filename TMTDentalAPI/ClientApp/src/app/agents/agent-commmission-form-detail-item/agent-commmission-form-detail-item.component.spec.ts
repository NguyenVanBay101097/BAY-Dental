import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AgentCommmissionFormDetailItemComponent } from './agent-commmission-form-detail-item.component';

describe('AgentCommmissionFormDetailItemComponent', () => {
  let component: AgentCommmissionFormDetailItemComponent;
  let fixture: ComponentFixture<AgentCommmissionFormDetailItemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AgentCommmissionFormDetailItemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgentCommmissionFormDetailItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
