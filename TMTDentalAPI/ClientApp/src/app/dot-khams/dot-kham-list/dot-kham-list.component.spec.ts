import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DotKhamListComponent } from './dot-kham-list.component';

describe('DotKhamListComponent', () => {
  let component: DotKhamListComponent;
  let fixture: ComponentFixture<DotKhamListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DotKhamListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DotKhamListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
