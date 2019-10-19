import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IrRuleListComponent } from './ir-rule-list.component';

describe('IrRuleListComponent', () => {
  let component: IrRuleListComponent;
  let fixture: ComponentFixture<IrRuleListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IrRuleListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IrRuleListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
