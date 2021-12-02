import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AgePopoverComponent } from './age-popover.component';

describe('AgePopoverComponent', () => {
  let component: AgePopoverComponent;
  let fixture: ComponentFixture<AgePopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AgePopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgePopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
