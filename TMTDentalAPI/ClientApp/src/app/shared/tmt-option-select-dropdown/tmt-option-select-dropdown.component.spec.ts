import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TmtOptionSelectDropdownComponent } from './tmt-option-select-dropdown.component';

describe('TmtOptionSelectDropdownComponent', () => {
  let component: TmtOptionSelectDropdownComponent;
  let fixture: ComponentFixture<TmtOptionSelectDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TmtOptionSelectDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TmtOptionSelectDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
