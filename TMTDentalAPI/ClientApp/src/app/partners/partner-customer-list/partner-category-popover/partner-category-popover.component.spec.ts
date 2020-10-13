import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCategoryPopoverComponent } from './partner-category-popover.component';

describe('PartnerCategoryPopoverComponent', () => {
  let component: PartnerCategoryPopoverComponent;
  let fixture: ComponentFixture<PartnerCategoryPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCategoryPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCategoryPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
